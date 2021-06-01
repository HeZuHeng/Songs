using UnityEngine;
using System.Collections;

public class TextNameItem : MonoBehaviour
{
    private TextMesh textName;

    Transform nameTran;
    Transform cameraTran;

    static Font font;
    void Update()
    {
        if(textName != null)
        {
            Vector3 cameraDirection = cameraTran.forward;
            cameraDirection.y = 0f;
            nameTran.rotation = Quaternion.LookRotation(cameraDirection);
        }
    }

    public void SetName(string name)
    {
        if (textName != null) textName.text = name;
    }

    public void AddName(string name,Vector3 pos,Transform parent)
    {
        nameTran = new GameObject("TextItemName").transform;
        nameTran.SetParent(parent);
        nameTran.position = pos;
        textName = nameTran.gameObject.AddComponent<TextMesh>();
        cameraTran = CameraMng.MainCamera.transform;
        if ("绅士".Equals(name))
        {
            textName.text = "Politician";
        }
        if ("政治家".Equals(name))
        {
            textName.text = "Politician";
        }
        else if ("船员B".Equals(name))
        {
            textName.text = "Sailor B";
        }
        else if ("难民作家".Equals(name))
        {
            textName.text = "The Writer";
        }
        else if ("女厨师".Equals(name))
        {
            textName.text = "Female Chef";
        }
        else if ("农夫".Equals(name))
        {
            textName.text = "The farmer";
        }
        else
        {
            textName.text = name;
        }
        textName.characterSize = 0.15f;
        textName.anchor = TextAnchor.LowerCenter;
        if(font == null)
        {
            font = Resources.Load<Font>("20210105234048111320");
        }
        textName.font = font;
    }

}