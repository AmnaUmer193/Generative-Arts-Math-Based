using UnityEngine;
using UnityEngine.UI;

public class RandomMathArt : MonoBehaviour
{
    public RawImage displayImage;
    public int textureWidth = 256;
    public int textureHeight = 256;

    public Button GenerateBtn;

    [Header("Fractal Settings")]
    [Range(1, 30)]
    public int maxIterations = 5;
    [Range(5.0f, 50.0f)]
    public float zoom = 5.0f;
    public Vector2 offset = new Vector2(0f, 0f);

    public enum FractalType { Mandelbrot, Julia, BurningShip }
    public FractalType selectedFractal = FractalType.Mandelbrot;

    [Range(2.0f, 10.0f)]
    public float escapeRadius = 2.0f;

    public Vector2 minBounds = new Vector2(-2f, -2f);
    public Vector2 maxBounds = new Vector2(2f, 2f);

    [Header("Color Settings")]
    public Gradient colorGradient;

    [Header("Julia Set Parameters")]
    public float cReal = -0.7f;
    public float cImaginary = 0.27015f;

    void Start()
    {
        GenerateBtn.onClick.AddListener(Generate);
    }

    Texture2D GenerateFractalArt(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        float aspectRatio = (float)width / height;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float realPart = Mathf.Lerp(minBounds.x, maxBounds.x, (float)x / width);
                float imaginaryPart = Mathf.Lerp(minBounds.y, maxBounds.y, (float)y / height);

                int iterations = CalculateFractal(realPart, imaginaryPart);

                Color color;
                if (iterations == maxIterations)
                {
                    color = Color.black;
                }
                else
                {
                    float t = (float)iterations / maxIterations;
                    color = colorGradient.Evaluate(t);
                }

                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    int CalculateFractal(float realPart, float imaginaryPart)
    {
        switch (selectedFractal)
        {
            case FractalType.Mandelbrot:
                return CalculateMandelbrot(realPart, imaginaryPart);
            case FractalType.Julia:
                return CalculateJulia(realPart, imaginaryPart);
            case FractalType.BurningShip:
                return CalculateBurningShip(realPart, imaginaryPart);
            default:
                return 0;
        }
    }

    int CalculateMandelbrot(float realPart, float imaginaryPart)
    {
        float zReal = 0f;
        float zImaginary = 0f;
        int iterations = 0;

        while (zReal * zReal + zImaginary * zImaginary <= escapeRadius * escapeRadius && iterations < maxIterations)
        {
            float tempReal = zReal * zReal - zImaginary * zImaginary + realPart;
            zImaginary = 2 * zReal * zImaginary + imaginaryPart;
            zReal = tempReal;

            iterations++;
        }

        return iterations;
    }
    int CalculateJulia(float realPart, float imaginaryPart)
    {
        float zReal = realPart;
        float zImaginary = imaginaryPart;
        int iterations = 0;

        while (zReal * zReal + zImaginary * zImaginary <= escapeRadius * escapeRadius && iterations < maxIterations)
        {
            float tempReal = zReal * zReal - zImaginary * zImaginary + cReal;
            zImaginary = 2 * zReal * zImaginary + cImaginary;
            zReal = tempReal;

            iterations++;
        }

        return iterations;
    }
    int CalculateBurningShip(float realPart, float imaginaryPart)
    {
        float zReal = realPart;
        float zImaginary = imaginaryPart;
        int iterations = 0;

        while (zReal * zReal + zImaginary * zImaginary <= escapeRadius * escapeRadius && iterations < maxIterations)
        {
            float tempReal = zReal * zReal - zImaginary * zImaginary + realPart;
            zImaginary = Mathf.Abs(2 * zReal * zImaginary) + imaginaryPart;
            zReal = tempReal;

            iterations++;
        }

        return iterations;
    }

    public void Generate()
    {
        Texture2D texture = GenerateFractalArt(textureWidth, textureHeight);
        displayImage.texture = texture;
    }

    private void OnValidate()
    {
        Generate();
    }
}
