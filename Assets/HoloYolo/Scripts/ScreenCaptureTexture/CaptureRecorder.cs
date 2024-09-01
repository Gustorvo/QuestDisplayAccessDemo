using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics; 

public class CaptureRecorder : MonoBehaviour
{
    void CreateVideoFromImages(string ffmpegPath, string imageDirectory, string outputVideoPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = $"-framerate 30 -i {imageDirectory}/frame_%04d.png -c:v libx264 -pix_fmt yuv420p {outputVideoPath}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();
        }
    }
}
