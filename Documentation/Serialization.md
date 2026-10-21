# Galaxy serialization

`GalaxySerializer` is the convenient attribute-driven API. `GalaxyBitWriter` and
`GalaxyBitReader` are the zero-overhead API for hand-written hot paths. Both use
the same compact primitives: ZigZag varints, bit-packed booleans, and optional
default values.

## Define a packet

```csharp
[GalaxySerializable]
public sealed class LoginPacket
{
    [GalaxyField(0, MaxLength = 32)] public string Name;
    [GalaxyField(1)] public int Version;
    [GalaxyField(2)] public GalaxyVector3 Position;
}
```

Explicit field orders are recommended for long-lived network contracts. Orders
must be unique and between 0 and 63. New fields can safely be appended with a
new order. Inserting or removing a field between existing fields is not wire
compatible because this format intentionally does not carry protobuf-style tags.

For messages passed directly to GalaxyCore networking methods, derive from
`GalaxyMessage`. The base class implements `INetworkSerrializeble`, so callers do
not need to invoke the serializer manually or add `[GalaxySerializable]`:

```csharp
public sealed class FireMessage : GalaxyMessage
{
    [GalaxyField(0)] public int WeaponId;
    [GalaxyField(1)] public GalaxyVector3 Target;
}

entity.SendMessage(10, new FireMessage
{
    WeaponId = 4,
    Target = target
});

FireMessage message = GalaxyMessage.Deserialize<FireMessage>(payload);
```

`GalaxyMessage.Register<T>()`, `TryDeserialize<T>()`, `DeserializeInto()`, and
`GetSchemaHash<T>()` are convenience forwards to the corresponding
`GalaxySerializer` APIs. Plain attributed classes remain useful for models that
should not implement the network-message interface.

`OmitDefault` is `true` by default. Zero, `false`, and `null` fields only consume
their presence bit. `MaxLength` limits UTF-8 bytes for strings and element count
for `byte[]`, arrays, and `List<T>`, during both serialization and deserialization.
For collections of strings or byte arrays, `MaxElementLength` independently
limits each string's UTF-8 byte length or each inner byte array's length.

## Optional quantization

Quantization is opt-in and has no configurable wire parameters:

```csharp
[GalaxySerializable]
public sealed class MovementPacket
{
    [GalaxyField(0), GalaxyQuantized]
    public float ApproximateValue; // IEEE binary16: 2 bytes

    [GalaxyField(1), GalaxyQuantizedNormalize]
    public GalaxyVector3 Direction; // normalized octahedral direction: 4 bytes

    [GalaxyField(2), GalaxyQuantizedNormalize]
    public GalaxyQuaternion Rotation; // normalized smallest-three rotation: 4 bytes
}
```

`GalaxyQuantizedNormalize` intentionally discards vector magnitude and must not
be used for positions, scales, or velocities whose length matters. A zero or
non-finite vector/quaternion cannot be normalized and is rejected when present.
The attributes also apply to nullable values, arrays, and `List<T>` elements.

GalaxyVars supports the same opt-in normalized encoding through the existing
attribute, without changing its constructor or the surrounding API:

```csharp
[GalaxyVar(1)]
public GalaxyVector3 Position; // lossless: 12 bytes

[GalaxyVar(2, QuantizedNormalize = true)]
public GalaxyVector3 Direction; // normalized direction: 4 bytes

[GalaxyVar(3, QuantizedNormalize = true)]
public GalaxyQuaternion Rotation; // normalized rotation: 4 bytes
```

As above, the vector form discards magnitude. Leave the flag disabled for
positions, scales, and other vectors whose length is meaningful.

For tracked collections, use `GalaxyVarArray<T>` instead of a raw array. Its
mutation version lets GalaxyVars detect an unchanged collection in O(1), without
scanning or keeping a second copy:

```csharp
[GalaxyVar(4)]
public readonly GalaxyVarArray<int> Scores = new GalaxyVarArray<int>(16);

[GalaxyVar(5, QuantizedNormalize = true)]
public readonly GalaxyVarArray<GalaxyQuaternion> Rotations =
    new GalaxyVarArray<GalaxyQuaternion>();

Scores[3] = 100;       // tracked automatically
Rotations.Add(rotation);
Rotations.RemoveAt(0);
```

The indexer, `Add`, `AddRange`, `Insert`, `Remove`, `RemoveAt`, `Resize`, `Clear`,
and `CopyFrom` track mutations. Capacity grows geometrically like `List<T>`, while
only `Count` logical elements are serialized. For a bulk hot path,
`DangerousGetArray()` exposes the capacity storage; only indices below `Count` are
logical elements, and `MarkDirty()` must be called once after direct mutations.
The wrapper must be initialized before GalaxyVars registration and cannot be
`null`. A changed collection currently sends a full snapshot; unchanged polling
never walks its elements.

GalaxyVars field IDs are local to a byte-sized scope. Registration without an
explicit scope keeps the original behavior and uses scope zero:

```csharp
GalaxyVars.RegistrationClass(playerState);       // default scope 0
GalaxyVars.RegistrationClass(1, combatState);    // fields default to scope 1
GalaxyVars.RegistrationClass(2, inventoryState); // fields default to scope 2
```

This allows multiple component models to reuse the same local field IDs. A peer
with a different class layout can override the registration scope per field:

```csharp
public sealed class FlattenedServerState
{
    [GalaxyVar(1, 0)] public GalaxyVector3 Position; // wire address (0, 1)
    [GalaxyVar(1, 1)] public int Health;             // wire address (1, 1)
    [GalaxyVar(1, 2)] public int Gold;               // wire address (2, 1)
}
```

Therefore client and server classes do not need matching composition; only the
resolved `(scope, fieldId)` wire addresses and value codecs must match. Explicit
field scope wins over the scope supplied to `RegistrationClass`.

## Serialize and deserialize

```csharp
Span<byte> destination = stackalloc byte[256];
int bitLength = GalaxySerializer.Serialize(packet, destination);
int byteLength = (bitLength + 7) >> 3;

LoginPacket copy = GalaxySerializer.Deserialize<LoginPacket>(
    destination.Slice(0, byteLength), bitLength);
```

Attributed objects can be mixed with hand-written fields in the same bitstream
without temporary buffers, copies, alignment, or an additional outer frame:

```csharp
writer.Write(messageCode);
GalaxySerializer.Serialize(packet, ref writer);
writer.Write(sequence);

byte code = reader.ReadByte();
LoginPacket packet = GalaxySerializer.Deserialize<LoginPacket>(ref reader);
uint sequence = reader.ReadUInt32();
```

The serializer uses the same primitive wire methods as the raw reader/writer.
An attributed object's own presence mask and collection/nested framing are still
part of its contract.

The convenience overload returns an exactly-sized array:

```csharp
byte[] payload = GalaxySerializer.Serialize(packet);
```

For pooled packet objects and reduced-allocation reads:

```csharp
GalaxySerializer.DeserializeInto(payload, pooledPacket, bitLength);

if (!GalaxySerializer.TryDeserializeInto(payload, pooledPacket, bitLength))
{
    // Truncated or malformed payload. The reused object can be partially modified.
}
```

`TryDeserialize<T>` creates a new object and returns `false` for malformed data.
`DeserializeInto` also reuses existing nested objects, same-sized arrays, and
`List<T>` instances. Lists are resized in place and nested collection elements
are reused by index when possible.

## Untrusted network payload limits

Deserialization uses `GalaxySerializerLimits.Default` automatically. It caps the
payload, strings, byte arrays, collection element counts, and nested-object depth
before allocating. These guards do not change the wire format. A field's
`MaxLength`/`MaxElementLength` and the global limit are both enforced; the
stricter value wins.

Use tighter limits for a particular endpoint when its packet shape is known:

```csharp
var limits = new GalaxySerializerLimits(
    maxPayloadBytes: 4096,
    maxStringByteLength: 128,
    maxByteArrayLength: 2048,
    maxArrayLength: 256,
    maxNestingDepth: 8);

if (!GalaxySerializer.TryDeserialize<LoginPacket>(payload, out var packet, limits))
{
    // Malformed or outside the endpoint's allocation budget.
}
```

`GalaxySerializerLimits.Unbounded` is available for trusted offline data. It is
not recommended for bytes received from the network.

## Warmup and schema diagnostics

```csharp
GalaxySerializer.Register<LoginPacket>();
ulong schemaHash = GalaxySerializer.GetSchemaHash<LoginPacket>();
```

Registration is optional. It builds the contract before the first packet and,
when JIT is available, compiles direct encode/decode delegates. On IL2CPP and
NativeAOT the cached reflection fallback is used. Unity IL2CPP projects must
also preserve reflected packet members with `[Preserve]` or `link.xml`.

The schema hash is diagnostic metadata and is not written into every packet.

## Custom codecs

Unsupported application types can be registered once and then used as normal
`[GalaxyField]` members, nullable values, or array elements:

```csharp
GalaxySerializer.RegisterCodec<MyPoint>(
    schemaId: 0x4D59504F494E5431UL,
    writer: WritePoint,
    reader: ReadPoint);

static void WritePoint(ref GalaxyBitWriter writer, MyPoint value)
{
    writer.Write(value.X);
    writer.Write(value.Y);
}

static MyPoint ReadPoint(ref GalaxyBitReader reader, MyPoint existing) =>
    new MyPoint(reader.ReadInt32(), reader.ReadInt32());
```

Register codecs before the first `Register<T>`, serialization, or schema-hash
request for a packet using that type. `schemaId` participates in the packet
schema hash and must change whenever the custom wire format changes. The reader
receives an existing value, allowing custom reference-type codecs to reuse
pooled instances. Registered delegates work on JIT, NativeAOT, and IL2CPP; the
wire format remains the codec author's responsibility.

## Supported fields

- Boolean, integer, floating-point, and enum types
- Nullable value types such as `int?`, enum?, and `GalaxyVector3?`
- `string` and `byte[]`
- `GalaxyVector3`, `GalaxyVectorD3`, `GalaxyQuaternion`, `GalaxyPose`,
  `GalaxyRay`, `GalaxyPlane`, and `GalaxyAabb`
- Nested classes marked with `[GalaxySerializable]` or derived from `GalaxyMessage`
- One-dimensional arrays and `List<T>` of any supported element type

For nullable fields, `null` is omitted through the normal presence mask, while
`0`, `false`, and enum zero remain present values. Arrays of nullable values are
not currently supported.

Nested objects are length-delimited bit frames. Adding fields to a nested
contract therefore remains compatible with an older reader: unknown trailing
data is skipped before reading the next field of the containing packet. The
frame uses a fixed 24-bit bit-length header so it can be written directly into
the destination without a temporary buffer.

Arrays and lists preserve the difference between `null` and an empty collection.
Their element count is encoded as a compact varuint, and equivalent `T[]` and
`List<T>` fields have identical wire bytes. Reference-type elements (`string`,
`byte[]`, and nested objects) may also be `null`; one presence bit is written per
element. `byte[]` remains a special densely encoded field and does not write a
presence bit for every byte, so `byte[][]` and `List<byte[]>` are supported.
Nested collections and multidimensional arrays such as `int[][]`,
`List<List<int>>`, and `int[,]` are rejected.

Use `GalaxyBitWriter` and `GalaxyBitReader` directly when the last few
nanoseconds matter or a packet needs custom bit ranges and quantization.
