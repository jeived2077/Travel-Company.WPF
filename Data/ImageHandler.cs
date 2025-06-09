using Microsoft.Win32;
using System.IO;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Data;

internal static class ImageHandler
{
    public static byte[]? ChangeProfilePicture(Client client)
    {
        // Пример: открытие диалога выбора файла и конвертация в byte[]
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpg)|*.png;*.jpg"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            return System.IO.File.ReadAllBytes(openFileDialog.FileName);
        }
        return null;
    }

    private static string GetFilter()
    {
        return
            "All Files (*.*)|*.*" +
            "|All Pictures (*.emf;*.wmf;*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.bmp;*.dib;*.rle;*.gif;*.emz;*.wmz;*.tif;*.tiff;*.svg;*.ico)" +
                "|*.emf;*.wmf;*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.bmp;*.dib;*.rle;*.gif;*.emz;*.wmz;*.tif;*.tiff;*.svg;*.ico" +
            "|Windows Enhanced Metafile (*.emf)|*.emf" +
            "|Windows Metafile (*.wmf)|*.wmf" +
            "|JPEG File Interchange Format (*.jpg;*.jpeg;*.jfif;*.jpe)|*.jpg;*.jpeg;*.jfif;*.jpe" +
            "|Portable Network Graphics (*.png)|*.png" +
            "|Bitmap Image File (*.bmp;*.dib;*.rle)|*.bmp;*.dib;*.rle" +
            "|Compressed Windows Enhanced Metafile (*.emz)|*.emz" +
            "|Compressed Windows MetaFile (*.wmz)|*.wmz" +
            "|Tag Image File Format (*.tif;*.tiff)|*.tif;*.tiff" +
            "|Scalable Vector Graphics (*.svg)|*.svg" +
            "|Icon (*.ico)|*.ico";
    }
}