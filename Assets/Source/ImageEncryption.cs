using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageEncryption : MonoBehaviour
{
    public Texture2D image;

    void Start()
    {
        // Load the image
        byte[] imageData = image.EncodeToPNG();

        // Encrypt the image data
        byte[] encryptedData = AESEncryption.Encrypt(imageData);
        File.WriteAllBytes("Assets/encryptedImage.bytes", encryptedData);
        Debug.Log("Image Encrypted and Saved!");

        // Decrypt the image data
        byte[] loadedEncryptedData = File.ReadAllBytes("Assets/encryptedImage.dat");
        byte[] decryptedData = AESEncryption.Decrypt(loadedEncryptedData);

        // Load the decrypted image back into a Texture2D
        Texture2D decryptedImage = new Texture2D(1, 1);
        decryptedImage.LoadImage(decryptedData);
        Debug.Log("Image Decrypted!");

        // Use the decrypted image (e.g., assign it to a material)
        GetComponent<Image>().sprite = Sprite.Create(decryptedImage, new Rect(Vector2.zero, new Vector2(decryptedImage.width, decryptedImage.height)), Vector2.zero);
    }
}