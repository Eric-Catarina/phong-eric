using UnityEngine;

public class PhongRenderer : MonoBehaviour
{
    public Transform fonteDeLuz;
    public Color corDaLuz = Color.white;
    public float intensidadeDaLuz = 1.0f;

    public Color corDosObjetos = Color.red;

    // esses 3 são multiplicadores da intensidade de cada componente
    public float multiplicadorDeCorAmbiente = 0.1f;
    public float multiplicadorDeCorDifusa = 0.7f;
    public float multiplicadorDeCorEspecular = 0.5f;


    // 
    public float tamanhoDoBrilhoEspecular = 32.0f;
    public GameObject quadroParaRenderizar;

    private Camera mainCamera;
    private Texture2D renderTexture;

    void Start()
    {
        mainCamera = Camera.main;
        renderTexture = new Texture2D(mainCamera.pixelWidth, mainCamera.pixelHeight);
        RenderScene();
        quadroParaRenderizar.GetComponent<Renderer>().material.mainTexture = renderTexture;
    }

    void OnGUI()
    {
        if (renderTexture != null)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTexture);
        }
    }

    void Update()
    {
    }

    void RenderScene()
    {
        for (int y = 0; y < renderTexture.height; y++)
        {
            for (int x = 0; x < renderTexture.width; x++)
            {
                Ray ray = mainCamera.ScreenPointToRay(new Vector3(x, y, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 pontoDeIntersecao = hit.point;
                    Vector3 normal = hit.normal;

                    Color finalColor = CalculateLighting(pontoDeIntersecao, normal);
                    renderTexture.SetPixel(x, y, finalColor);

                    // Aqui ele vai desenhar um raio de debug para visualizar a normal
                    Debug.DrawRay(pontoDeIntersecao, normal * 0.5f, Color.green, 9999.0f);
                }
                else
                {
                    renderTexture.SetPixel(x, y, Color.black);
                }
            }
        }
        renderTexture.Apply();
    }

    Color CalculateLighting(Vector3 point, Vector3 normal)
    {
        // A cor ambiente sempre vai ser aplicada se o raio atingir o objeto
        Color ambient = corDosObjetos * multiplicadorDeCorAmbiente;

        // A cor difusa depende apenas da luz e do ângulo entre a luz e a normal, não depende do ponto de vista do observador/camera
        Vector3 lightDirection = (fonteDeLuz.position - point).normalized;
        float diffuseAngle = Mathf.Max(0.0f, Vector3.Dot(normal, lightDirection));
        Color diffuse = corDosObjetos * corDaLuz * diffuseAngle * multiplicadorDeCorDifusa;

        // Já a cor especular depende do ponto de vista do observador/camera, além da luz e da normal
        Vector3 viewDirection = (mainCamera.transform.position - point).normalized;
        Vector3 reflectDirection = Vector3.Reflect(-lightDirection, normal);
        float specularAngle = Mathf.Pow(Mathf.Max(0.0f, Vector3.Dot(viewDirection, reflectDirection)), tamanhoDoBrilhoEspecular);
        Color specular = corDaLuz * specularAngle * multiplicadorDeCorEspecular;

        // A cor final do pixel é a soma das três componentes
        Color finalColor = ambient + diffuse + specular;
        return finalColor;
    }
}