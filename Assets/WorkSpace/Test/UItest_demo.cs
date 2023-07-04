
using UnityEngine;
using UnityEngine.UI;
using jagat.art;

public class UItest_demo : MonoBehaviour
{
    public Button btn_Add, btn_Del;
    private GameObject boy, girl, girl01;

    public Button btn_return, btn_H;
    private Text _Text;
    void Start()
    {

        _Text = GameObject.Find("TakePhotoAndDefault_Text").GetComponent<Text>();
       
        GameObject.FindObjectOfType<UserController>().type = UserController.ControllType.Default;
        _Text.text = "Default";
        btn_return.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });

        btn_Add.onClick.AddListener(() => {



            var room = GameObject.FindObjectOfType<InteractiveRoom>();


            if ((room as InteractiveRoom_LoveRoom) != null && boy == null)
            {
                Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_0;
                boy = ABLoader.instance.LoadBoy(parent);
            }
            else if ((room as InteractiveRoom_LoveRoom) != null && girl == null)
            {
                Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_1;

                girl = ABLoader.instance.LoadGirl(parent);

                room.SetRoomBrightness();
                room.SetParticleEffect();
                room.SetSpawnEffect();
                room.SetStarsAnimating();
            }



            if ((room as InteractiveRoom_MerryGoRound) != null && boy == null)
            {
                Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_0;
                boy = ABLoader.instance.LoadBoy(parent);
                (room as InteractiveRoom_MerryGoRound)?.RotatingMuma(1);
            }
            else if ((room as InteractiveRoom_MerryGoRound) != null && girl == null && boy != null)
            {
                Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_1;
                (room as InteractiveRoom_MerryGoRound)?.RotatingMuma(2);
                girl = ABLoader.instance.LoadGirl(parent);

                room.SetRoomBrightness();
                room.SetParticleEffect();
                room.SetSpawnEffect();
                room.SetStarsAnimating();
            }
            else if ((room as InteractiveRoom_MerryGoRound) != null && girl != null && boy != null && girl01 == null)
            {
                Transform parent01 = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_pisition_2;

                girl01 = ABLoader.instance.LoadGirl01(parent01);


            }




            if ((room as InteractiveRoom_ClassRoom) != null && boy == null)
            {
                Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_0;
                boy = ABLoader.instance.LoadBoy(parent);
            }
            else if ((room as InteractiveRoom_ClassRoom) != null && girl == null)
            {
                Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_1;
                Transform parent01 = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_pisition_2;
                girl = ABLoader.instance.LoadGirl(parent);
                girl01 = ABLoader.instance.LoadGirl01(parent01);
                room.SetRoomBrightness();
                room.SetParticleEffect();
                room.SetSpawnEffect();
                room.SetStarsAnimating();
            }

            if ((room as InteractiveRoom_KTVRoom) != null && boy == null)
            {
                Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_0;
                boy = ABLoader.instance.LoadBoy(parent);
            }
            else if ((room as InteractiveRoom_KTVRoom) != null && girl == null)
            {
                Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_1;
                Transform parent01 = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_pisition_2;
                girl = ABLoader.instance.LoadGirl(parent);
                girl01 = ABLoader.instance.LoadGirl01(parent01);
                room.SetRoomBrightness();
                room.SetParticleEffect();
                room.SetSpawnEffect();
                room.SetStarsAnimating();
            }




            //if (boy == null)
            //{
            //    Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_0;
            //    boy = ABLoader.instance.LoadBoy(parent);

            //    (room as InteractiveRoom_MerryGoRound)?.RotatingMuma(1);

            //}
            //else if (girl == null)
            //{

            //    Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_position_1;
            //    Transform parent01 = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_pisition_2;
            //    girl = ABLoader.instance.LoadGirl(parent);

            //    room.SetRoomBrightness();
            //    room.SetParticleEffect();
            //    room.SetSpawnEffect();
            //    room.SetStarsAnimating();
            //    (room as InteractiveRoom_MerryGoRound)?.RotatingMuma(2);

            //}
            //else if (girl01 == null)
            //{
            //    Transform parent = GameObject.FindObjectOfType<ActorGeneratePositions>().actor_pisition_2;
            //    girl01 = ABLoader.instance.LoadGirl01(parent);

            //    room.SetRoomBrightness();
            //    room.SetParticleEffect();
            //    room.SetSpawnEffect();
            //    room.SetStarsAnimating();
            //    (room as InteractiveRoom_MerryGoRound)?.RotatingMuma(2);
            //}


        });


        btn_Del.onClick.AddListener(() => {

            var room = GameObject.FindObjectOfType<InteractiveRoom>();



            if ((room as InteractiveRoom_LoveRoom) != null && girl != null)
            {
                boy.GetComponent<Animation>().CrossFade("LoveRoom_boy_idle");

                Destroy(girl);
                room.SetRoomBrightness(false);
                room.SetParticleEffect(false);
                room.SetStarsAnimating(false);
            }

            if ((room as InteractiveRoom_MerryGoRound) != null)
            {
                if (girl01 != null)
                {
                    boy.GetComponent<Animation>().CrossFade("Merry_go_round_two_02");
                    girl.GetComponent<Animation>().CrossFade("Merry_go_round_two_01");
                    Destroy(girl01);

                }
                else if (girl != null && girl01 == null)
                {
                    boy.GetComponent<Animation>().CrossFade("Merry_go_round_idel");

                    Destroy(girl);
                    room.SetRoomBrightness(false);
                    room.SetParticleEffect(false);
                    room.SetStarsAnimating(false);
                    (room as InteractiveRoom_MerryGoRound)?.RotatingMuma(1);
                }

            }
            if ((room as InteractiveRoom_ClassRoom) != null && girl != null)
            {

                boy.GetComponent<Animation>().CrossFade("ClassRoom_one_student_1");
                Destroy(girl01);
                Destroy(girl);
                room.SetRoomBrightness(false);
                room.SetParticleEffect(false);
                room.SetStarsAnimating(false);
            }
            if ((room as InteractiveRoom_KTVRoom) != null && girl != null)
            {

                boy.GetComponent<Animation>().CrossFade("ClassRoom_one_student_1");
                Destroy(girl01);
                Destroy(girl);
                room.SetRoomBrightness(false);
                room.SetParticleEffect(false);
                room.SetStarsAnimating(false);
            }




        });
        btn_H.onClick.AddListener(() => {
            var controller = GameObject.FindObjectOfType<UserController>();
            if (controller.type == UserController.ControllType.Default)
            {
                controller.type = UserController.ControllType.TakePhoto;
                _Text.text = "TakePhoto";
            }
            else
            {
                controller.type = UserController.ControllType.Default;
                _Text.text = "Default";
            }





        });
    }




    // Update is called once per frame
    void Update()
    {

    }
}
