using LiteNetLib;
using OnlineGame.Networking;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{   [Serializable]
    public class NetworkObject : MonoBehaviour
    {
        public bool IsLocal;
        public string Id { get { return playerData.PlayerID; } }
        //[SerializeField] string _playerPrefabName = "player";

        private Coroutine _updateCoroutine;
        private PlayerData playerData;

        public void Initialize(bool isLocal, PlayerData Data)
        {
            playerData = Data;
            IsLocal = isLocal;
            if (Data.Transform.Transform != null) //Restore old transform
            {
                transform.SetPositionAndRotation(Utility.PosToVector3(Data.Transform.Transform.Position),
                                                 Utility.RotToQuaternion(Data.Transform.Transform.Rotation));
            }
            else
            {
                transform.position = new Vector3(UnityEngine.Random.Range(-5, 5), transform.position.y,
                                                 UnityEngine.Random.Range(-5, 5));
            }

            if (isLocal)
            {
                GetComponent<PlayerController>().StartLocaleInputs();
                NetworkManager.LocalPlayerObject = this;
                StartLocaleUpdate();
            }
            else
            {
                NewPosition = transform.position;
                NewRotation = transform.rotation;
                if (Data.Transform.Transform != null)
                    OnTransfrormReceive(Data.Transform);
            }
        }

        public void StartLocaleUpdate() //Запускаем отправку на сервер, проверяя не начали ли мы её уже
        {
            if (_updateCoroutine==null)
                _updateCoroutine = StartCoroutine(UpdateLocaleTransformOnServer());
        }

        public void StopUpdate()
        {
            if (_updateCoroutine != null)
            {
                StopCoroutine(_updateCoroutine);
                _updateCoroutine = null;
            }
        }

        IEnumerator UpdateLocaleTransformOnServer()
        {
            while (true)
            {
                var netTransform = new Transform(Utility.Vector3ToPos(transform.position), 
                                                 Utility.QuaternionToRot(transform.rotation));
                //var netObject = new TransformData(Id, SceneManager.GetActiveScene().name, _playerPrefabName, netTransform);
                playerData.Transform.Transform = netTransform;
                NetworkManager.Instance.CurrentRoom.SendEventFromRoom(PacketType.Transfrom, playerData, DeliveryMethod.Unreliable);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void OnTransfrormReceive(TransformData data) //если это не локальный клиент
        {
            Debug.Log("Receive remote transform" + Utility.PosToVector3(data.Transform.Position));

            NewPosition = Utility.PosToVector3(data.Transform.Position);
            NewRotation = Utility.RotToQuaternion(data.Transform.Rotation);
        }

        private Vector3 NewPosition;
        private Quaternion NewRotation;

        [SerializeField, Range(1,100)] private float smoothTick = 5; // 0.02 * 5


        private void Update()
        {
            if (IsLocal)
                return;

            InterpotaleMovements();
        }

        private void InterpotaleMovements()
        {
            if (transform.position == NewPosition && transform.rotation == NewRotation)
                return;

            var smooth = Time.deltaTime * smoothTick;
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, NewPosition, smooth), 
                                             Quaternion.Lerp(transform.rotation, NewRotation, smooth));
        }
    }
}
