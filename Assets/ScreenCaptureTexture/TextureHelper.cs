using UnityEngine;

public class TextureHelper
{
	//https://gamedev.stackexchange.com/questions/203539/rotating-a-unity-texture2d-90-180-degrees-without-using-getpixels32-or-setpixels
	public static void FlipImageVerticallyCPU(Texture2D tex)
	{
		int width = tex.width;
		int height = tex.height;

		var texels = tex.GetRawTextureData<Color32>();
		var copy = System.Buffers.ArrayPool<Color32>.Shared.Rent(texels.Length);
		Unity.Collections.NativeArray<Color32>.Copy(texels, copy, texels.Length);

		int address = 0;
		for (int newY = 0; newY < height; newY++)
		{
			for (int newX = 0; newX < width; newX++)
			{
				int oldX = newX;
				int oldY = height - newY - 1;

				texels[address++] = copy[oldY * width + oldX];
			}
		}

		System.Buffers.ArrayPool<Color32>.Shared.Return(copy);
	}

	public static void FlipImageHorizontallyCPU(Texture2D tex)
	{
		int width = tex.width;
		int height = tex.height;

		// Retrieve raw texture data
		var texels = tex.GetRawTextureData<Color32>();

		// Create a copy of the texture data using an array pool to save memory allocations
		var copy = System.Buffers.ArrayPool<Color32>.Shared.Rent(texels.Length);
		Unity.Collections.NativeArray<Color32>.Copy(texels, copy, texels.Length);

		int address = 0;

		// Iterate over each pixel in the texture
		for (int newY = 0; newY < height; newY++)
		{
			for (int newX = 0; newX < width; newX++)
			{
				// Calculate the index for the horizontally flipped position
				int oldX = width - newX - 1; // Flip horizontally
				int oldY = newY; // Keep the Y-coordinate the same

				// Copy the pixel from the flipped position in the original image
				texels[address++] = copy[oldY * width + oldX];
			}
		}

		// Return the rented array to the pool
		System.Buffers.ArrayPool<Color32>.Shared.Return(copy);
	}


	public static void FlipImageVerticallyAndHorizontallyCPU(Texture2D tex)
	{
		int width = tex.width;
		int height = tex.height;

		// Retrieve raw texture data
		var texels = tex.GetRawTextureData<Color32>();

		// Create a copy of the texture data using an array pool to save memory allocations
		var copy = System.Buffers.ArrayPool<Color32>.Shared.Rent(texels.Length);
		Unity.Collections.NativeArray<Color32>.Copy(texels, copy, texels.Length);

		int address = 0;

		// Iterate over each pixel in the texture
		for (int newY = 0; newY < height; newY++)
		{
			for (int newX = 0; newX < width; newX++)
			{
				// Calculate the indices for the horizontally and vertically flipped positions
				int oldX = width - newX - 1; // Flip horizontally
				int oldY = height - newY - 1; // Flip vertically

				// Copy the pixel from the flipped position in the original image
				texels[address++] = copy[oldY * width + oldX];
			}
		}

		// Return the rented array to the pool
		System.Buffers.ArrayPool<Color32>.Shared.Return(copy);
	}
}