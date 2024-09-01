using Unity.Sentis;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class RunYOLO : MonoBehaviour
{
	const string modelName = "yolov7-tiny";

	// Link the classes.txt here:
	public TextAsset labelsAsset;

	// Create a Raw Image in the scene and link it here:
	public RawImage displayImage;

	// Link to a bounding box texture here:
	public Sprite boxTexture;

	// Link to the font for the labels:
	public Font font;

	// Link to the video player here:
	public VideoPlayer videoPlayer;

	public Transform displayLocation;
	private Model runtimeModel;
	//private Worker engine;
	private string[] labels;
	private RenderTexture targetRT;
	const BackendType backend = BackendType.GPUCompute;

	//Image size for the model
	private const int imageWidth = 640;
	private const int imageHeight = 640;


	private readonly YoloVisualizer _yoloVisualizer;

	public RunYOLO()
	{
		_yoloVisualizer = new YoloVisualizer(this);
	}

	//bounding box data
	public struct BoundingBox
	{
		public float centerX;
		public float centerY;
		public float width;
		public float height;
		public string label;
		public float confidence;
	}

	public YoloVisualizer YoloVisualizer
	{
		get { return _yoloVisualizer; }
	}

	void Start()
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;

		//Parse neural net labels
		labels = labelsAsset.text.Split('\n');

		//Load model
		ModelAsset modelAsset = Resources.Load(modelName) as ModelAsset;
		runtimeModel = ModelLoader.Load(modelAsset);

		targetRT = new RenderTexture(imageWidth, imageHeight, 0);

		//Create image to display video
		displayLocation = displayImage.transform;

		//Create engine to run model
		//engine = new Worker(runtimeModel, backend);

		videoPlayer.Play();
	}

	private void Update()
	{
		//ExecuteML();
	}

	public void ExecuteML()
	{
		YoloVisualizer.ClearAnnotations();

		if (videoPlayer && videoPlayer.texture)
		{
			float aspect = videoPlayer.width * 1f / videoPlayer.height;
			Graphics.Blit(videoPlayer.texture, targetRT, new Vector2(1f / aspect, 1), new Vector2(0, 0));
			displayImage.texture = targetRT;
		}
		else return;

		using var input = TextureConverter.ToTensor(targetRT, imageWidth, imageHeight, 3);
		
		// from documentation: Remove uses of worker.Execute or worker.SetInputs with a dictionary, instead use an array or set the inputs one at a time by name.
		//engine.Schedule(input);

		//Read output tensors
		//var output = engine.PeekOutput() as Tensor<float>;
		//output.MakeReadable(); // uncommented because method doesn't exist in 2.0.0 (replaced by smth else?)

		float displayWidth = displayImage.rectTransform.rect.width;
		float displayHeight = displayImage.rectTransform.rect.height;

		float scaleX = displayWidth / imageWidth;
		float scaleY = displayHeight / imageHeight;

		//Draw the bounding boxes
		// for (int n = 0; n < output.shape[0]; n++)
		// {
		// 	var box = new BoundingBox
		// 	{
		// 		centerX = ((output[n, 1] + output[n, 3]) * scaleX - displayWidth) / 2,
		// 		centerY = ((output[n, 2] + output[n, 4]) * scaleY - displayHeight) / 2,
		// 		width = (output[n, 3] - output[n, 1]) * scaleX,
		// 		height = (output[n, 4] - output[n, 2]) * scaleY,
		// 		label = labels[(int)output[n, 5]],
		// 		confidence = Mathf.FloorToInt(output[n, 6] * 100 + 0.5f)
		// 	};
		// 	YoloVisualizer.DrawBox(box, n);
		// }
	}

	private void OnDestroy()
	{
		//engine?.Dispose();
	}
}