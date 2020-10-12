using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using UnityEngine;

/// <summary>
/// Компонент синхронизации параметров аниматора
/// версия 0.1 (использовать только под гнетом страха)
/// </summary>
public class GalaxyAnimSync : MonoBehaviour, IAnimatorSync
{
    /// <summary>
    /// Ссылкан а текущий аниматор
    /// </summary>
    private Animator animator;
    /// <summary>
    /// Синхронизатор
    /// </summary>
    private AnimatorSync animatorSync;
    /// <summary>
    /// Ссылка на сетевубю сущность
    /// </summary>
    private ClientNetEntity netEntity;
    /// <summary>
    /// Число параметров
    /// </summary>
    private int count;
    /// <summary>
    /// Имена параметров
    /// </summary>
    private string[] names;
    /// <summary>
    /// Кеш типов параметров
    /// </summary>
    private FastType[] types;
    /// <summary>
    /// Флоты для лерпа
    /// </summary>
    private float[] floatsLearp;
    /// <summary>
    /// Целевые флоты
    /// </summary>
    private float[] floatsTarget;
    void Start()
    {
        animator = GetComponent<Animator>();
        netEntity = GetComponent<UnityNetEntity>().netEntity;
        count = animator.parameterCount;
        types = new FastType[count];
        animatorSync = new AnimatorSync(netEntity, (byte)count, this);
        names = new string[count];
        floatsLearp = new float[count];
        floatsTarget = new float[count];
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.GetParameter(i).type == AnimatorControllerParameterType.Bool)
            {
                names[i] = animator.GetParameter(i).name;
                animatorSync.RegParameter(i, animator.GetBool(names[i]));
                types[i] = FastType._bool;
            }
            if (animator.GetParameter(i).type == AnimatorControllerParameterType.Float)
            {
                names[i] = animator.GetParameter(i).name;
                animatorSync.RegParameter(i, animator.GetFloat(names[i]));
                types[i] = FastType._float;
            }
            if (animator.GetParameter(i).type == AnimatorControllerParameterType.Int)
            {
                names[i] = animator.GetParameter(i).name;
                animatorSync.RegParameter(i, animator.GetInteger(names[i]));
                types[i] = FastType._int;
            }
        }
    }


    void OnEnable()
    {
        GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
    }
    void OnDisable()
    {
        GalaxyEvents.OnFrameUpdate -= OnFrameUpdate;
    }
    private void OnFrameUpdate()
    {
        if (!netEntity.isMy) return;
        for (int i = 0; i < count; i++)
        {
            switch (types[i])
            {
                case FastType._float:

                    animatorSync.SetValue(i, animator.GetFloat(names[i]));
                    break;
                case FastType._bool:
                    animatorSync.SetValue(i, animator.GetBool(names[i]));
                    break;

                case FastType._int:
                    animatorSync.SetValue(i, animator.GetInteger(names[i]));
                    break;
                default:
                    break;
            }
        }
        animatorSync.Sync();
    }

    public void SetBool(int id, bool value)
    {
        animator.SetBool(names[id], value);
    }

    public void SetFloat(int id, float value)
    {
        floatsTarget[id] = value;
    }

    public void SetInt(int id, int value)
    {
        animator.SetInteger(names[id], value);
    }

    public void SetTrigger(int id, bool value)
    {
        // animator.SetTrigger(names[id]);
    }

    void Update()
    {
        if (netEntity.isMy) return;
        for (int i = 0; i < count; i++)
        {
            if (types[i] == FastType._float)
            {
                floatsLearp[i] = Mathf.Lerp(floatsLearp[i], floatsTarget[i], GalaxyApi.lerpDelta);
                animator.SetFloat(names[i], floatsLearp[i]);
            }
        }
    }
}

/// <summary>
/// Типы переменных для хеша
/// </summary>
public enum FastType : byte
{
    _none = 0,
    _float = 1,
    _bool = 2,
    _trigger = 3,
    _int = 4,
}
