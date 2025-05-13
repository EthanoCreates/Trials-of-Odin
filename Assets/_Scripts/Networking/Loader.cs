using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        LobbyScene, 
        MidgardScene
    }

    public static Scene targetScene;

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallBack()
    {
        //one frame after using call back we load the target scene
        SceneManager.LoadScene(targetScene.ToString());
    }
}
