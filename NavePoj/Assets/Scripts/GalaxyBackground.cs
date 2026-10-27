
using UnityEngine;

public class GalaxyBackground : MonoBehaviour
{
    [Header("Galaxia")]
    [Range(2, 6)] public int spiralArms = 4;
    [Range(500, 5000)] public int starCount = 2500;
    [Range(100, 1000)] public int gasCount = 600;

    public float galaxyRadius = 8f;
    public float spiralTightness = 0.85f;
    public float rotationSpeed = 1.2f;

    [Header("Aparencia")]
    public Color backgroundColor = new Color(0.008f, 0.012f, 0.045f);
    public Color starColor = new Color(0.75f, 0.85f, 1f);
    public Color blueGas = new Color(0.15f, 0.4f, 1f);
    public Color purpleGas = new Color(0.55f, 0.15f, 1f);
    public Color pinkGas = new Color(1f, 0.2f, 0.65f);
    public Color centerColor = new Color(1f, 0.85f, 1f);

    [Header("Camera")]
    public Camera mainCamera;
    public bool changeCameraBackground = true;

    private ParticleSystem stars;
    private ParticleSystem gas;
    private ParticleSystem core;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (changeCameraBackground && mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = backgroundColor;
        }

        CreateGalaxy();
    }

    void Update()
    {
        transform.Rotate(
            0f,
            0f,
            rotationSpeed * Time.deltaTime
        );
    }

    void CreateGalaxy()
    {
        gas = CreateSystem("GasClouds", 0);
        stars = CreateSystem("Stars", 1);
        core = CreateSystem("GalaxyCore", 2);

        GenerateGas();
        GenerateStars();
        GenerateCore();
    }

    ParticleSystem CreateSystem(string objectName, int order)
    {
        GameObject obj = new GameObject(objectName);
        obj.transform.SetParent(transform, false);

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.maxParticles = Mathf.Max(starCount, gasCount) + 100;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.startLifetime = 10000f;
        main.startSpeed = 0f;

        var emission = ps.emission;
        emission.enabled = false;

        var shape = ps.shape;
        shape.enabled = false;

        var renderer = obj.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = -100 + order;

        // Material e textura criados automaticamente
        Material material = new Material(
            Shader.Find("Sprites/Default")
        );

        material.mainTexture = CreateSoftTexture(
            objectName == "GasClouds" ? 128 : 64,
            objectName == "GasClouds"
        );

        renderer.material = material;

        return ps;
    }

    Texture2D CreateSoftTexture(int resolution, bool cloudy)
    {
        Texture2D texture = new Texture2D(
            resolution,
            resolution,
            TextureFormat.RGBA32,
            false
        );

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dx = (x - resolution / 2f) / (resolution / 2f);
                float dy = (y - resolution / 2f) / (resolution / 2f);

                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                float alpha = Mathf.Clamp01(1f - distance);

                if (cloudy)
                {
                    float noise = Mathf.PerlinNoise(
                        x * 0.055f,
                        y * 0.055f
                    );

                    alpha = Mathf.Pow(alpha, 2.5f) *
                            Mathf.Lerp(0.35f, 1f, noise);
                }
                else
                {
                    alpha = Mathf.Pow(alpha, 4f);
                }

                texture.SetPixel(
                    x, y,
                    new Color(1f, 1f, 1f, alpha)
                );
            }
        }

        texture.Apply();
        return texture;
    }

    Vector3 SpiralPosition(float radius, int arm, float variation)
    {
        float angle =
            arm * Mathf.PI * 2f / spiralArms +
            radius * spiralTightness +
            Random.Range(-variation, variation);

        return new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius,
            0f
        );
    }

    void GenerateStars()
    {
        ParticleSystem.Particle[] particles =
            new ParticleSystem.Particle[starCount];

        for (int i = 0; i < starCount; i++)
        {
            float radius = Mathf.Sqrt(Random.value) * galaxyRadius;

            int arm = Random.Range(0, spiralArms);

            Vector3 position = SpiralPosition(radius, arm, 0.28f);

            Color color = Color.Lerp(
                centerColor,
                starColor,
                radius / galaxyRadius
            );

            color.a = Random.Range(0.65f, 1f);

            particles[i].position = position;
            particles[i].startSize = Random.Range(0.025f, 0.065f);
            particles[i].startColor = color;
            particles[i].startLifetime = 10000f;
            particles[i].remainingLifetime = 10000f;
        }

        stars.SetParticles(particles, particles.Length);
    }

    void GenerateGas()
    {
        ParticleSystem.Particle[] particles =
            new ParticleSystem.Particle[gasCount];

        for (int i = 0; i < gasCount; i++)
        {
            float radius = Random.Range(1f, galaxyRadius);

            int arm = Random.Range(0, spiralArms);

            Vector3 position = SpiralPosition(radius, arm, 0.48f);

            Color[] colors = { blueGas, purpleGas, pinkGas };

            Color color = colors[Random.Range(0, colors.Length)];

            color.a = Random.Range(0.07f, 0.19f);

            particles[i].position = position;
            particles[i].startSize = Random.Range(0.25f, 0.65f);
            particles[i].startColor = color;
            particles[i].startLifetime = 10000f;
            particles[i].remainingLifetime = 10000f;
        }

        gas.SetParticles(particles, particles.Length);
    }

    void GenerateCore()
    {
        ParticleSystem.Particle[] particles =
            new ParticleSystem.Particle[100];

        for (int i = 0; i < particles.Length; i++)
        {
            float radius = Random.Range(0f, 1.1f);

            Vector2 offset = Random.insideUnitCircle * radius;

            Color color = Color.Lerp(
                centerColor,
                pinkGas,
                Random.value * 0.5f
            );

            color.a = Random.Range(0.1f, 0.35f);

            particles[i].position = new Vector3(
                offset.x,
                offset.y,
                0f
            );

            particles[i].startSize = Random.Range(0.2f, 0.55f);
            particles[i].startColor = color;
            particles[i].startLifetime = 10000f;
            particles[i].remainingLifetime = 10000f;
        }

        core.SetParticles(particles, particles.Length);
    }
}
