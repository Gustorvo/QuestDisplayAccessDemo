using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

public enum FlipMode
{
	None,
	Vertical,
	Horizontal,
	Both
}

public class ImageSaver : MonoBehaviour
{
	[SerializeField] int frequency = 1;
	[SerializeField] bool mirrorCopy = true;
	[SerializeField] int imageLimit = 200; // 0 = unlimited

	private string savePath;
	private Texture2D copyMirroredTexture;
	private byte[] reusableManagedArray; // Reusable managed array to minimize allocations

	private Texture2D bufferTexture = default;

	CancellationTokenSource cts = new CancellationTokenSource();
	private CancellationToken token;

	public void EnableImageSaver() => StartSavingTextureAsync();
	public void DisableImageSaver() => StopSavingTextureAsync();
	private Awaitable savingTask;
	ScreenCaptureTextureManager screenCapture;
	private float timeSinceLastSave = 0f;
	private bool shouldSave = false;
	private int count;

	private void Awake()
	{
		screenCapture = GetComponent<ScreenCaptureTextureManager>();
		Assert.IsNotNull(screenCapture);

		token = cts.Token;

		bufferTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false, false);
		savePath = Application.persistentDataPath + "/ScreenCapture/";

		if (!Directory.Exists(savePath))
		{
			Directory.CreateDirectory(savePath);
		}

		// start saving texture when screen capture starts
		screenCapture.OnScreenCaptureStarted.AddListener(StartSavingTextureAsync);

		// start saving texture if screen capture is already active
		if (screenCapture.ScreenCaptureIsActive) StartSavingTextureAsync();
	}

	private void StartSavingTextureAsync()
	{
		// check if saving is already in progress
		if (savingTask != null && !savingTask.IsCompleted) return;
		try
		{
			savingTask = StartEncodingAndSavingAsync();
		}
		catch (OperationCanceledException)
		{
			Debug.Log("Saving task was cancelled");
		}
	}

	private void StopSavingTextureAsync()
	{
		cts?.Cancel();
	}

	private async Awaitable StartEncodingAndSavingAsync()
	{
		while (isActiveAndEnabled)
		{
			if (count >= imageLimit) cts.Cancel();
			token.ThrowIfCancellationRequested();
			if (!screenCapture.ScreenCaptureIsActive)
			{
				// await until screen capture will be active
				await AwaitableHelper.WaitForConditionAsync(() =>
					screenCapture.ScreenCaptureIsActive, token);
			}

			// Wait for the specified frequency interval
			await Awaitable.WaitForSecondsAsync(frequency * 0.5f, token);

			await Save();
		}
	}

	private async Awaitable Save()
	{
		Graphics.CopyTexture(screenCapture.ScreenTexture, bufferTexture);

		// Capture and save the texture
		var firstTask = CaptureAndSaveTextureAsync(bufferTexture, FlipMode.Vertical);
		count++;

		// Check if a mirrored copy is needed
		if (mirrorCopy)
		{
			await firstTask;
			await Awaitable.WaitForSecondsAsync(frequency * 0.5f, token);
			await CaptureAndSaveTextureAsync(bufferTexture, FlipMode.Horizontal);
			count++;
		}
	}

	private async Awaitable CaptureAndSaveTextureAsync(Texture2D texture, FlipMode flipMode = FlipMode.None)
	{
		if (flipMode != FlipMode.None)
		{
			switch (flipMode)
			{
				case FlipMode.Vertical:
					TextureHelper.FlipImageVerticallyCPU(texture);
					break;
				case FlipMode.Horizontal:
					TextureHelper.FlipImageHorizontallyCPU(texture);
					break;
				case FlipMode.Both:
					TextureHelper.FlipImageVerticallyAndHorizontallyCPU(texture);
					break;
			}
		}

		// Save the bytes to a file asynchronously
		await SaveTextureToFileAsync(texture);
	}


	private async Awaitable SaveTextureToFileAsync(Texture2D texture)
	{
		// Asynchronously write to the file using the managed array
		string timeStampWithMilliseconds = System.DateTime.Now.ToString("dd-HH-mm-ss-fff");
		string fullPath = Path.Combine(savePath, $"{timeStampWithMilliseconds}_image.jpg");

		await using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None,
			             4096 * 4,
			             useAsync: true))
		{
			byte[] pngData = texture.EncodeToJPG();
			await fileStream.WriteAsync(pngData);
		}
	}

	private void OnDestroy()
	{
		if (copyMirroredTexture != null)
		{
			Destroy(copyMirroredTexture);
		}

		cts.Cancel();
		cts.Dispose();
		screenCapture.OnScreenCaptureStarted.RemoveListener(StartSavingTextureAsync);

	}
}