using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using System.IO;
using System.Text;

public class AnimationPlayerController : MonoBehaviour
{
    public Animator characterAnimator;
    public Transform character;

    public AnimationHolder animationHolder;

    [Header("UI Related Stuffs")]
    public GameObject scrollViewContent;
    public GameObject animationClipButton;
    public Slider animationSpeedSlider;
    public TextMeshProUGUI animationSpeedText;
    public TextMeshProUGUI selectedAnimationText;
    public Toggle deg90Check;
    public TMP_InputField commentField;
    public Button addCommentButton;

    private List<string[]> rowData = new List<string[]>();

    private int selectedAnimationClip;

    private void Start()
    {
        InstansiateButtonPerAnimationClip();
        ReadCSVData();
#if UNITY_EDITOR
        ClearOtherDataFromScriptable();
#endif
    }

    private void InstansiateButtonPerAnimationClip()
    {
        for (int i = 0; i < animationHolder.animationClips.Count; i++)
        {
            string x = animationHolder.animationClips[i].animationName;
            GameObject go = Instantiate(animationClipButton, scrollViewContent.transform);
            Button button = go.GetComponent<Button>();
            button.onClick.AddListener(() =>
                PlayAnimationOnButtonClick(x));
            go.transform.GetComponentInChildren<TextMeshProUGUI>().text = animationHolder.animationClips[i].animationName;
        }
    }

    private void ClearOtherDataFromScriptable()
    {
        for (int i = 0; i < animationHolder.animationClips.Count; i++)
        {
            animationHolder.animationClips[i].speed = 1f;
            animationHolder.animationClips[i].turnModelTo90Deg = false;
            animationHolder.animationClips[i].clipComment = "NA";
        }
    }

    public void PlayAnimationOnButtonClick(string animationId)
    {
        Debug.Log("Selected animation id : " + animationId);
        character.eulerAngles = new Vector3(
                character.rotation.x,
                0f,
                character.rotation.z
                );
        deg90Check.isOn = false;
        animationSpeedSlider.value = 1f;
        characterAnimator.runtimeAnimatorController = animationHolder.ReturnAnimationOverride(animationId).animationOverridePerClip;
        selectedAnimationClip = animationHolder.ReturnSelectedAnimationClipIndex(animationId);
        Debug.Log("Selected clip comment : " + animationHolder.animationClips[selectedAnimationClip].clipComment);
        commentField.text = animationHolder.animationClips[selectedAnimationClip].clipComment;
        selectedAnimationText.text = animationHolder.animationClips[selectedAnimationClip].animationName;
    }

    public void AdjustAnimationSpeed()
    {
        characterAnimator.speed = animationSpeedSlider.value;
        animationSpeedText.text = "Animation Speed: " + animationSpeedSlider.value.ToString("0.0");
        animationHolder.animationClips[selectedAnimationClip].speed = float.Parse(animationSpeedSlider.value.ToString("0.0"));
    }

    public void RotateCharacterTo90DegAngle()
    {
        if (deg90Check.isOn)
        {
            character.eulerAngles = new Vector3(
                character.rotation.x,
                -90f,
                character.rotation.z
                );
            animationHolder.animationClips[selectedAnimationClip].turnModelTo90Deg = deg90Check.isOn;
        }
        else
        {
            character.eulerAngles = new Vector3(
                character.rotation.x,
                0f,
                character.rotation.z
                );
            animationHolder.animationClips[selectedAnimationClip].turnModelTo90Deg = deg90Check.isOn;
        }
    }

    public void AddCommentToSpecificAnimation()
    {
        StartCoroutine(CommentAdded());
    }

    private IEnumerator CommentAdded()
    {
        addCommentButton.interactable = false;
        animationHolder.animationClips[selectedAnimationClip].clipComment = commentField.text;
        string tempText = animationHolder.animationClips[selectedAnimationClip].clipComment;
        commentField.text = "Comment is added to this animation";
        yield return new WaitForSeconds(0.5f);
        commentField.text = tempText;
        addCommentButton.interactable = true;
    }

    public void ExportAnimationScriptableValues()
    {

        string[] headerRowData = new string[] { "Animation Name", "Animation Speed", "Is90DegRotation", "Comments" };
        rowData.Add(headerRowData);

        for (int i = 0; i < animationHolder.animationClips.Count; i++)
        {
            string[] innerRowData = new string[] {
                animationHolder.animationClips[i].animationName,
                animationHolder.animationClips[i].speed.ToString("0.0"),
                animationHolder.animationClips[i].turnModelTo90Deg.ToString(),
                animationHolder.animationClips[i].clipComment
            };
            rowData.Add(innerRowData);
        }

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        string filePath = Application.streamingAssetsPath + "/ExportedValueOfAnimations.csv";
        if (File.Exists(Application.streamingAssetsPath + "/ExportedValueOfAnimations.csv"))
            File.WriteAllText(Application.streamingAssetsPath + "/ExportedValueOfAnimations.csv", string.Empty);

        StreamWriter outStream = File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();

    }

    private void ReadCSVData()
    {
        if (!File.Exists(Application.streamingAssetsPath + "/ExportedValueOfAnimations.csv"))
        {
            Debug.LogError("File is not present in system");
            return;
        }

        StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/ExportedValueOfAnimations.csv");

        List<string> commentsList = new List<string>();
        int i = 0;
        while (!reader.EndOfStream)
        {
            string lines = reader.ReadLine();
            if (!string.IsNullOrEmpty(lines))
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                
                Debug.Log("Lines : " + lines);

                string[] commentVal = lines.Split(',');
                commentsList.Add(commentVal[3]);
                Debug.Log("Comment Val : " + commentVal[3]);
                animationHolder.animationClips[i - 1].clipComment = commentVal[3];
            }
            i++;
        }

    }
    
}
