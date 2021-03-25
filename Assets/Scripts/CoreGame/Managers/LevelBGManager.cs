using UnityEngine;

public class LevelBGManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem bgCubes;

    private Color[] camBGs = new Color[]
    {
        new Color(0.03426486f, 0.09433961f, 0.07932092f),
        new Color(0.03921569f, 0.06666667f, 0.1019608f),
        new Color(0.08059642f, 0.03921569f, 0.1019608f),
        new Color(0.1037736f, 0.07015281f, 0.03279637f),
        new Color(0.08490568f, 0.03163938f, 0.03829767f)
    };

    private Color[,] cubesBGs = new Color[,]
    {
        { new Color(0.01779993f, 0.1509434f, 0.02429472f), new Color(0.09019607f, 0.3960784f, 0.1317819f) },
        { new Color(0.02758988f, 0.1177999f, 0.1886792f), new Color(0.09158064f, 0.2361584f, 0.3962264f) },
        { new Color(0.136436f, 0.05900675f, 0.245283f), new Color(0.378567f, 0.09019607f, 0.3960784f) },
        { new Color(0.2641509f, 0.1687544f, 0.04610181f), new Color(0.3960784f, 0.3313709f, 0.09019607f) },
        { new Color(0.1320755f, 0.01156192f, 0.004360985f), new Color(0.3301887f, 0.1074671f, 0.1532318f) }
    };

    public void SetLvlBG(int size)
    {
        size -= 2;

        GetComponent<Camera>().backgroundColor = camBGs[size];
        ParticleSystem.MainModule mainModule = bgCubes.main;
        Gradient grad = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[2];
        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        gck[0].color = cubesBGs[size, 0];
        gck[0].time = 0f;
        gak[0].alpha = 1f;
        gak[0].time = 0f;
        gck[1].color = cubesBGs[size, 1];
        gck[1].time = 1f;
        gak[1].alpha = 1f;
        gak[1].time = 1f;
        grad.SetKeys(gck, gak);
        mainModule.startColor = new ParticleSystem.MinMaxGradient(grad);

        bgCubes.Stop();
        bgCubes.Clear();
        bgCubes.Play();
    }
}