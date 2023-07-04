
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ABLoader : MonoBehaviour
{

    /// <summary>
    /// 衣服部件对应身体的遮罩图
    /// </summary>
    private Dictionary<string, string> maskDic = new Dictionary<string, string> { 
        //衣服
        {"TOPS00","UB_Mask_01_duanxiu" },
        {"TOPS02","UB_Mask_03_beixin" },
        {"TOPS03","UB_Mask_02_changxiu" },
        {"TOPS04","UB_Mask_02_changxiu" },
        {"TOPS05","UB_Mask_02_changxiu" },
        {"TOPS07","UB_Mask_01_duanxiu" },
        {"TOPS16","UB_Mask_01_duanxiu" },
        {"TOPS17","UB_Mask_01_duanxiu" },
        {"TOPS19","UB_Mask_01_duanxiu" },
        {"TOPS20","UB_Mask_01_duanxiu" },
        {"TOPS25","UB_Mask_03_beixin" },
        {"TOPS26","UB_Mask_01_duanxiu" },
        {"TOPS27","UB_Mask_02_changxiu" },
        //裤子
        {"BOTTOM00","LB_Mask_01_duanku" },
        {"BOTTOM02","LB_Mask_01_duanku" },
        {"BOTTOM03","LB_Mask_01_duanku" },
        {"BOTTOM04","LB_Mask_01_duanku" },
        {"BOTTOM05","LB_Mask_01_changku" },
        {"BOTTOM07","LB_Mask_01_changku" },
        {"BOTTOM16","LB_Mask_01_duanku" },
        {"BOTTOM17","LB_Mask_01_duanku" },
        {"BOTTOM19","LB_Mask_01_duanku" },
        {"BOTTOM20","LB_Mask_01_duanku" },
        {"BOTTOM25","LB_Mask_01_duanku" },
        {"BOTTOM26","LB_Mask_01_changku" },
        {"BOTTOM27","LB_Mask_01_changku" },
        {"BOTTOM627","LB_Mask_01_changku" },
        //连衣裙
        {"DRESS04","LB_Mask_01_changqun04" },
        {"DRESS05","LB_Mask_01_changqun05" },
        {"DRESS09","LB_Mask_01_changqun09" },
        {"DRESS10","LB_Mask_01_duanku" },
    };



    static AssetBundle ab;
    static AssetBundle mask;

    private GameObject boy,girl;
    public static ABLoader instance;
    void Start()
    {
        instance = this;

        if (ab != null) return;

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/avatar_win");
            mask = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/bodymask_win");
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/avatar_android");
            mask = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/bodymask_android");
        }


    }



    public GameObject LoadBoy(Transform parent)
    {


         var prefab=  ab.LoadAsset<GameObject>("AvatarSkeleton_boy");
         boy=   Instantiate<GameObject>(prefab,parent);
        boy.transform.localPosition = Vector3.zero;
        boy.transform.localRotation = Quaternion.identity;
        boy.transform.localScale = Vector3.one*1f;

        var body = boy.transform.Find("SkinRoot/BODY_ST");
        var mat = body.GetComponent<SkinnedMeshRenderer>().material;
        mat.SetTexture("_MaskTexture_1", mask.LoadAsset<Texture2D>( maskDic["TOPS17"]));
        mat.SetTexture("_MaskTexture_2", mask.LoadAsset<Texture2D>(maskDic["BOTTOM02"]));
        mat.SetTexture("_MaskTexture_3", null);
        mat.SetTexture("_MaskTexture_4", null);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("muma"))
        {
            boy.GetComponent<Animation>().Play("Merry_go_round_idel");
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("love"))
        {
            boy.GetComponent<Animation>().Play("LoveRoom_boy_idle");
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("class"))
        {
            boy.GetComponent<Animation>().Play("ClassRoom_one_student_1");
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("KTV"))
        {
            boy.GetComponent<Animation>().Play("ClassRoom_one_student_1");
        }

        return boy;

    }

    public GameObject LoadGirl(Transform parent)
    {
        var prefab = ab.LoadAsset<GameObject>("AvatarSkeleton_girl");
         girl = Instantiate<GameObject>(prefab, parent);
        girl.transform.localPosition = Vector3.zero;
        girl.transform.localRotation = Quaternion.identity;
        girl.transform.localScale = Vector3.one*1f;

        var body = girl.transform.Find("SkinRoot/BODY_ST");
        var mat = body.GetComponent<SkinnedMeshRenderer>().material;
        mat.SetTexture("_MaskTexture_1", mask.LoadAsset<Texture2D>(maskDic["TOPS16"]));
        mat.SetTexture("_MaskTexture_2", mask.LoadAsset<Texture2D>(maskDic["BOTTOM16"]));
        mat.SetTexture("_MaskTexture_3", null);
        mat.SetTexture("_MaskTexture_4", null);


        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("muma"))
        {
           
            girl.GetComponent<Animation>().Play("Merry_go_round_two_01");
            boy.GetComponent<Animation>().CrossFade("Merry_go_round_two_02");
        }

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("love"))
        {
            girl.GetComponent<Animation>().Play("LoveRoom_girl_lover");
            boy.GetComponent<Animation>().CrossFade("LoveRoom_boy_lover");
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("KTV"))
        {
            girl.GetComponent<Animation>().Play("LoveRoom_girl_lover");
            boy.GetComponent<Animation>().CrossFade("LoveRoom_boy_lover");
        }

        return girl;
    }
    public GameObject LoadGirl01(Transform parent)
    {
        var prefab = ab.LoadAsset<GameObject>("AvatarSkeleton_girl");
        var girl01 = Instantiate<GameObject>(prefab, parent);
        girl01.transform.localPosition = Vector3.zero;
        girl01.transform.localRotation = Quaternion.identity;
        girl01.transform.localScale = Vector3.one * 1f;

        var body = girl01.transform.Find("SkinRoot/BODY_ST");
        var mat = body.GetComponent<SkinnedMeshRenderer>().material;
        mat.SetTexture("_MaskTexture_1", mask.LoadAsset<Texture2D>(maskDic["TOPS16"]));
        mat.SetTexture("_MaskTexture_2", mask.LoadAsset<Texture2D>(maskDic["BOTTOM16"]));
        mat.SetTexture("_MaskTexture_3", null);
        mat.SetTexture("_MaskTexture_4", null);


        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("muma"))
        {

            girl01.GetComponent<Animation>().Play("Merry_go_round_three_1");
            boy.GetComponent<Animation>().CrossFade("Merry_go_round_three_2");
            girl.GetComponent<Animation>().CrossFade("Merry_go_round_three_3");
        };


        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("class"))
        {
          
            boy.GetComponent<Animation>().CrossFade("ClassRoom_three_student_1");
            girl.GetComponent<Animation>().CrossFade("ClassRoom_three_student_2");
            girl01.GetComponent<Animation>().Play("ClassRoom_three_student_3");


        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("KTV"))
        {

            boy.GetComponent<Animation>().CrossFade("ClassRoom_three_student_1");
            girl.GetComponent<Animation>().CrossFade("ClassRoom_three_student_2");
            girl01.GetComponent<Animation>().Play("ClassRoom_three_student_3");


        }

        return girl01;
    }
}