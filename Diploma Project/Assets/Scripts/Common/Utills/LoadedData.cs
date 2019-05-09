using UnityEngine;


public abstract class LoadedData<T>
{
    #region Fields

    protected Coroutine loadingCorutine;
    protected ICoroutineInfo target;

    #endregion



    #region Properties

    public bool IsStartLoading
    {
        get;
        protected set;
    } = false;


    public bool IsLoaded
    {
        get;
        protected set;
    } = false;


    public T Data
    {
        get;
        protected set;
    }

    #endregion



    #region Public methods

    public abstract void StartLoadData(string url, ICoroutineInfo target);


    public abstract void StopLoadData();

    #endregion
}


public class LoadedTexture : LoadedData<Texture>
{
    #region Public methods

    public override void StartLoadData(string url, ICoroutineInfo target)
    {
        IsStartLoading = true;
        loadingCorutine = GlobalServerManager.Instance.LoadTexture(url, (isSuccess, texture) =>
        {
            Data = texture;
            IsLoaded = true;
        });
    }


    public override void StopLoadData()
    {
        if (loadingCorutine != null)
        {
            target.CoroutineStop(loadingCorutine);
        }
        loadingCorutine = null;
    }

    #endregion
}
