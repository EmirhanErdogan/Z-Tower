using UnityEngine;
using Cinemachine;
using DG.Tweening;

namespace Emir
{
    public class CameraManager : Singleton<CameraManager>
    {
        #region Serializable Fields

        [Header("General")] [SerializeField] private Camera m_camera;
        [SerializeField] private CinemachineVirtualCamera m_virtualCamera;
        private bool IsShake;

        #endregion

        /// <summary>
        /// This function returns related camera component.
        /// </summary>
        /// <returns></returns>
        public Camera GetCamera()
        {
            return m_camera;
        }

        /// <summary>
        /// This function returns related virtual camera component.
        /// </summary>
        /// <returns></returns>
        public CinemachineVirtualCamera GetVirtualCamera()
        {
            return m_virtualCamera;
        }

        public void CameraShake()
        {
            if (IsShake is true) return;
            GetCamera().DOShakePosition(0.15f, 0.05f, 5,10f).OnStart(() => { IsShake = true; }).OnComplete(() =>
            {
                IsShake = false;
            });
        }
    }
}