using Microsoft.Win32;
using System.IO;
using Travel_Company.WPF.Models;

namespace Travel_Company.WPF.Data;

internal static class ImageHandler
{
    public static void ChangeProfilePicture(Client client)
    {
        var dialog = new OpenFileDialog
        {
            Filter = GetFilter()
        };
        if (dialog.ShowDialog() == true)
        {
            var filename = dialog.FileName;
            using var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream);
            var fileData = binaryReader.ReadBytes((int)fileStream.Length);
            client.Photograph = fileData;
        }
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