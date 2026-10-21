using System;
using System.Collections.Generic;
using GalaxyCoreCommon.NetEntities;
using GalaxyCoreCommon.Serialization;
using GalaxyCoreLib.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    /// <summary>
    /// Synchronizes Animator parameters without knowing anything about a specific controller.
    ///
    /// Drop this component next to UnityNetEntity and an Animator. At runtime it reads the
    /// AnimatorController parameter list, sorts it by name and assigns compact deterministic IDs.
    /// The owner sends only changed values. Observers apply those values to their local Animator.
    ///
    /// This component intentionally synchronizes parameters, not animation clips or normalized
    /// state time. Every client still evaluates the same AnimatorController locally; the network
    /// only supplies the small set of inputs which drive that state machine.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GalaxyNetwork.Core.Scripts.NetEntity.UnityNetEntity))]
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(32000)]
    public sealed class GalaxyAnimSync : MonoBehaviour
    {
        [Tooltip("Small float noise below this value is not a meaningful animation change and is not sent.")]
        [Min(0f)]
        [SerializeField] private float floatThreshold = 0.001f;

        [Tooltip("After changes, a reliable full snapshot heals a missed unreliable cosmetic delta. Unchanged animators do not keep sending snapshots.")]
        [Min(0.1f)]
        [SerializeField] private float fullSnapshotInterval = 2f;

        [Tooltip("Parameters driven by animation curves already evaluate locally and normally should not be sent.")]
        [SerializeField] private bool ignoreCurveDrivenParameters = true;

        [Tooltip("Print one clear warning when two clients use different Animator parameter schemas.")]
        [SerializeField] private bool warnOnSchemaMismatch = true;

        private sealed class ParameterBinding
        {
            public uint Id;
            public string Name;
            public int Hash;
            public AnimatorControllerParameterType Type;
            public float FloatValue;
            public int IntValue;
            public bool BoolValue;
            public bool TriggerWasSet;
            public bool TriggerQueued;
        }

        private GalaxyNetwork.Core.Scripts.NetEntity.UnityNetEntity _view;
        private Animator _animator;
        private ClientNetEntity _subscribedEntity;
        private ParameterBinding[] _bindings = Array.Empty<ParameterBinding>();
        private Dictionary<int, ParameterBinding> _byHash = new Dictionary<int, ParameterBinding>();

        private int _dirtyCount;
        private int[] _dirtyIds = Array.Empty<int>();
        private byte[] _writeBuffer = Array.Empty<byte>();
        private uint _schemaHash;
        private uint _revision;
        private uint _lastSnapshotRevision;
        private float _nextSnapshotAt;
        private bool _forceSnapshot = true;
        private bool _schemaWarningPrinted;

        private void Awake()
        {
            _view = GetComponent<GalaxyNetwork.Core.Scripts.NetEntity.UnityNetEntity>();
            _animator = GetComponent<Animator>();
            BuildSchema();
            UpdateBinding();
        }

        private void OnEnable() => UpdateBinding();
        private void OnDisable() => UpdateBinding();

        private void Update()
        {
            UpdateBinding();

            if (_subscribedEntity == null || !_subscribedEntity.IsInit || !_subscribedEntity.IsMy)
                return;

            int changedCount = CaptureChangedParameters();
            bool snapshotDue = _forceSnapshot ||
                (Time.unscaledTime >= _nextSnapshotAt && _revision != _lastSnapshotRevision);

            // A snapshot excludes trigger pulses; send the dirty delta first so no pulse is lost.
            if (snapshotDue && changedCount > 0) SendState(false);
            if (snapshotDue)
            {
                SendState(true);
                _forceSnapshot = false;
                _lastSnapshotRevision = _revision;
                _nextSnapshotAt = Time.unscaledTime + fullSnapshotInterval;
            }
            else if (changedCount > 0)
            {
                SendState(false);
            }
        }

        /// <summary>
        /// Preferred way to fire a trigger when code can access this component.
        /// Direct Animator.SetTrigger calls are also detected in Update in normal Unity execution,
        /// but explicitly queuing the trigger removes any dependency on Animator evaluation order.
        /// </summary>
        public void SetTrigger(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("Animator trigger name cannot be empty.", nameof(parameterName));
            SetTrigger(Animator.StringToHash(parameterName));
        }

        /// <summary>Hash overload for code which already caches Animator.StringToHash.</summary>
        public void SetTrigger(int parameterHash)
        {
            _animator.SetTrigger(parameterHash);
            if (_byHash.TryGetValue(parameterHash, out ParameterBinding binding) &&
                binding.Type == AnimatorControllerParameterType.Trigger)
            {
                binding.TriggerQueued = true;
            }
        }

        /// <summary>
        /// Forces the next owner update to send all persistent parameters reliably.
        /// Useful after replacing an AnimatorController at runtime.
        /// </summary>
        public void ForceFullSnapshot()
        {
            _forceSnapshot = true;
        }

        private void BuildSchema()
        {
            AnimatorControllerParameter[] source = _animator.parameters;
            Array.Sort(source, CompareParameters);

            var bindings = new List<ParameterBinding>(source.Length);
            _byHash.Clear();

            for (int i = 0; i < source.Length; i++)
            {
                AnimatorControllerParameter parameter = source[i];
                if (ignoreCurveDrivenParameters && _animator.IsParameterControlledByCurve(parameter.nameHash))
                    continue;

                var binding = new ParameterBinding
                {
                    Id = (uint)bindings.Count,
                    Name = parameter.name,
                    Hash = parameter.nameHash,
                    Type = parameter.type
                };
                ReadCurrentValue(binding);
                bindings.Add(binding);
                _byHash[binding.Hash] = binding;
            }

            _bindings = bindings.ToArray();
            _dirtyIds = new int[_bindings.Length];

            // The buffer is reused while writing. SendAnimatorState serializes synchronously, so
            // only the final exact-sized byte[] is allocated for an outgoing packet.
            int estimatedBytes = 16 + _bindings.Length * 8;
            _writeBuffer = new byte[Mathf.Min(GalaxyAnimatorStatePacket.MaxPayloadLength,
                Mathf.Max(256, estimatedBytes))];
            _schemaHash = CalculateSchemaHash(_bindings);
        }

        private static int CompareParameters(AnimatorControllerParameter left,
            AnimatorControllerParameter right)
        {
            int byName = string.CompareOrdinal(left.name, right.name);
            return byName != 0 ? byName : ((int)left.type).CompareTo((int)right.type);
        }

        private int CaptureChangedParameters()
        {
            int dirtyCount = 0;

            for (int i = 0; i < _bindings.Length; i++)
            {
                ParameterBinding binding = _bindings[i];
                bool changed = false;

                switch (binding.Type)
                {
                    case AnimatorControllerParameterType.Float:
                    {
                        float value = _animator.GetFloat(binding.Hash);
                        if (Mathf.Abs(value - binding.FloatValue) > floatThreshold)
                        {
                            binding.FloatValue = value;
                            changed = true;
                        }
                        break;
                    }
                    case AnimatorControllerParameterType.Int:
                    {
                        int value = _animator.GetInteger(binding.Hash);
                        if (value != binding.IntValue)
                        {
                            binding.IntValue = value;
                            changed = true;
                        }
                        break;
                    }
                    case AnimatorControllerParameterType.Bool:
                    {
                        bool value = _animator.GetBool(binding.Hash);
                        if (value != binding.BoolValue)
                        {
                            binding.BoolValue = value;
                            changed = true;
                        }
                        break;
                    }
                    case AnimatorControllerParameterType.Trigger:
                    {
                        // Triggers are pulses, not persistent state. We send only the rising edge.
                        // DefaultExecutionOrder places this poll after normal gameplay Update code,
                        // while SetTrigger() above offers a completely explicit path when required.
                        bool isSet = _animator.GetBool(binding.Hash);
                        changed = binding.TriggerQueued || (isSet && !binding.TriggerWasSet);
                        binding.TriggerQueued = false;
                        binding.TriggerWasSet = isSet;
                        break;
                    }
                }

                if (!changed)
                    continue;
                _dirtyIds[dirtyCount++] = i;
                _revision++;
            }

            _dirtyCount = dirtyCount;
            return dirtyCount;
        }

        private void SendState(bool fullSnapshot)
        {
            var writer = GalaxyBitBuffer.ForWriting(_writeBuffer);

            // Packet format:
            // byte version | byte flags | uint schema hash | varuint entry count |
            // repeated: varuint parameter id + value inferred from the shared schema.
            writer.WriteByte(GalaxyAnimatorStatePacket.CurrentVersion);
            writer.WriteByte(fullSnapshot ? GalaxyAnimatorStatePacket.FullSnapshotFlag : (byte)0);
            writer.WriteUInt32(_schemaHash);

            int count = fullSnapshot ? CountPersistentParameters() : CountDirtyParameters();
            writer.WriteVarUInt32((uint)count);

            if (fullSnapshot)
            {
                for (int i = 0; i < _bindings.Length; i++)
                {
                    if (_bindings[i].Type != AnimatorControllerParameterType.Trigger)
                        WriteParameter(ref writer, _bindings[i]);
                }
            }
            else
            {
                int dirtyCount = CountDirtyParameters();
                for (int i = 0; i < dirtyCount; i++)
                    WriteParameter(ref writer, _bindings[_dirtyIds[i]]);
            }

            // Galaxy transport currently accepts an exact byte[]. Keeping the reusable write
            // buffer avoids growth allocations; this small final copy is the convenience-layer
            // trade-off. Performance-critical games can build a specialized adapter later.

            _dirtyCount = 0;
            _subscribedEntity.SendAnimatorState(writer.WrittenSpan.ToArray());
        }

        private int CountDirtyParameters()
        {
            return _dirtyCount;
        }

        private int CountPersistentParameters()
        {
            int count = 0;
            for (int i = 0; i < _bindings.Length; i++)
                if (_bindings[i].Type != AnimatorControllerParameterType.Trigger)
                    count++;
            return count;
        }

        private static void WriteParameter(ref GalaxyBitWriter writer, ParameterBinding binding)
        {
            writer.WriteVarUInt32(binding.Id);
            switch (binding.Type)
            {
                case AnimatorControllerParameterType.Float:
                    writer.WriteSingle(binding.FloatValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    writer.WriteVarInt32(binding.IntValue);
                    break;
                case AnimatorControllerParameterType.Bool:
                    writer.WriteByte(binding.BoolValue ? (byte)1 : (byte)0);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    // Presence of the ID is the pulse; no additional boolean is necessary.
                    break;
            }
        }

        private void OnAnimatorStateReceived(byte[] payload)
        {
            if (_subscribedEntity == null || _subscribedEntity.IsMy || payload == null)
                return;

            try
            {
                var reader = GalaxyBitBuffer.From(payload);
                byte version = reader.ReadByte();
                reader.ReadByte(); // Flags matter to transport reliability, not to application.
                uint remoteSchema = reader.ReadFixedUInt32();

                if (version != GalaxyAnimatorStatePacket.CurrentVersion || remoteSchema != _schemaHash)
                {
                    WarnSchemaMismatch(remoteSchema);
                    return;
                }

                uint count = reader.ReadUInt32();
                if (count > (uint)_bindings.Length)
                    throw new InvalidOperationException("Animator packet contains too many parameters.");

                for (uint i = 0; i < count; i++)
                {
                    uint id = reader.ReadUInt32();
                    if (id >= (uint)_bindings.Length)
                        throw new InvalidOperationException("Animator packet contains an unknown parameter id.");
                    ApplyParameter(ref reader, _bindings[id]);
                }

                if (!reader.End)
                    throw new InvalidOperationException("Animator packet contains trailing data.");
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Dropped malformed Galaxy animator state: " + exception.Message, this);
            }
        }

        private void ApplyParameter(ref GalaxyBitReader reader, ParameterBinding binding)
        {
            switch (binding.Type)
            {
                case AnimatorControllerParameterType.Float:
                    binding.FloatValue = reader.ReadSingle();
                    _animator.SetFloat(binding.Hash, binding.FloatValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    binding.IntValue = reader.ReadInt32();
                    _animator.SetInteger(binding.Hash, binding.IntValue);
                    break;
                case AnimatorControllerParameterType.Bool:
                    binding.BoolValue = reader.ReadByte() != 0;
                    _animator.SetBool(binding.Hash, binding.BoolValue);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    _animator.SetTrigger(binding.Hash);
                    break;
            }
        }

        private void ReadCurrentValue(ParameterBinding binding)
        {
            switch (binding.Type)
            {
                case AnimatorControllerParameterType.Float:
                    binding.FloatValue = _animator.GetFloat(binding.Hash);
                    break;
                case AnimatorControllerParameterType.Int:
                    binding.IntValue = _animator.GetInteger(binding.Hash);
                    break;
                case AnimatorControllerParameterType.Bool:
                    binding.BoolValue = _animator.GetBool(binding.Hash);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    binding.TriggerWasSet = _animator.GetBool(binding.Hash);
                    break;
            }
        }

        private void UpdateBinding()
        {
            ClientNetEntity current = isActiveAndEnabled && _view != null ? _view.NetEntity : null;
            if (ReferenceEquals(current, _subscribedEntity))
                return;

            if (_subscribedEntity != null)
            {
                _subscribedEntity.AnimatorStateReceived -= OnAnimatorStateReceived;
                _subscribedEntity.OnChangeOwner -= OnOwnerChanged;
            }

            _subscribedEntity = current;
            _forceSnapshot = true;
            _nextSnapshotAt = 0f;

            if (_subscribedEntity != null)
            {
                _subscribedEntity.AnimatorStateReceived += OnAnimatorStateReceived;
                _subscribedEntity.OnChangeOwner += OnOwnerChanged;
            }
        }

        private void OnOwnerChanged(int ownerId, byte[] customData)
        {
            // A new owner must establish a fresh baseline. Observers will receive this full packet
            // reliably before depending on later unreliable cosmetic deltas.
            _forceSnapshot = true;
            _nextSnapshotAt = 0f;
            for (int i = 0; i < _bindings.Length; i++)
                ReadCurrentValue(_bindings[i]);
        }

        private void WarnSchemaMismatch(uint remoteSchema)
        {
            if (!warnOnSchemaMismatch || _schemaWarningPrinted)
                return;
            _schemaWarningPrinted = true;
            Debug.LogWarning(
                "GalaxyAnimSync ignored a packet because Animator parameter schemas differ. " +
                "Make sure owner and observer prefabs use the same controller. Local=" +
                _schemaHash + ", remote=" + remoteSchema, this);
        }

        private static uint CalculateSchemaHash(ParameterBinding[] bindings)
        {
            const uint offset = 2166136261u;
            const uint prime = 16777619u;
            uint hash = offset;

            for (int i = 0; i < bindings.Length; i++)
            {
                string name = bindings[i].Name;
                for (int c = 0; c < name.Length; c++)
                {
                    char value = name[c];
                    hash = (hash ^ (byte)value) * prime;
                    hash = (hash ^ (byte)(value >> 8)) * prime;
                }
                hash = (hash ^ (byte)bindings[i].Type) * prime;
            }
            return hash;
        }

        private void OnDestroy()
        {
            if (_subscribedEntity == null)
                return;
            _subscribedEntity.AnimatorStateReceived -= OnAnimatorStateReceived;
            _subscribedEntity.OnChangeOwner -= OnOwnerChanged;
        }
    }
}
