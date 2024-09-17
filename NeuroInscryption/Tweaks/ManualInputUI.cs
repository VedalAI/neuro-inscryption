using System.Linq;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using GBC;
using JetBrains.Annotations;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeuroInscryption.Tweaks;

public sealed class ManualInputUI : Singlyboi<ManualInputUI>
{
    public sealed class TextBehaviour : ViewportRelativePosition
    {
        private static string Text => $"MANUAL INPUT ENABLED - Current flow: <color=#eef4c6>{AutomationManager.CurrentFlowName}</color>";

        private float _swapTimer = 0.5f;
        private PixelText[] _texts = null!;

        private new void Awake()
        {
            UpdateCamera();
            base.Awake();

            _texts = [transform.GetChild(0).GetComponentInChildren<PixelText>(), transform.GetChild(1).GetComponentInChildren<PixelText>()];
        }

        public override void OnEnable()
        {
            base.OnEnable();

            foreach (PixelText text in _texts)
            {
                text.SetText(Text);
            }

            foreach (PixelText text in _texts)
            {
                text.gameObject.SetActive(true);
            }
        }

        public override void ManagedUpdate()
        {
            UpdateCamera();
            base.ManagedUpdate();

            _swapTimer -= Time.deltaTime;

            foreach (PixelText text in _texts)
            {
                text.SetText(Text);
            }

            if (_swapTimer <= 0)
            {
                _swapTimer = 0.5f;
                foreach (PixelText text in _texts)
                {
                    text.gameObject.SetActive(!text.gameObject.activeSelf);
                }
            }
        }

        private void UpdateCamera()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (viewportCam && sceneName == "Part1_Cabin" && viewportCam.name == "OrthographicUICamera") return;

            Camera targetCamera = sceneName == "Part1_Cabin" ? FindObjectsOfType<Camera>().First(c => c.name == "OrthographicUICamera") : Camera.main!;

            viewportCam = targetCamera;
        }
    }

    private GameObject _textBehaviour = null!;

    // ReSharper disable once Unity.IncorrectMethodSignature
    [UsedImplicitly]
    private async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == "Ascension_Configure");

        SequentialPixelTextLines foundText = UnityEngine.Resources.FindObjectsOfTypeAll<SequentialPixelTextLines>()
            .First(t => t.name == "Footer" && t.transform.parent.name == "ChallengesScreen");

        GameObject obj = Instantiate(foundText.transform.gameObject, transform);
        obj.SetActive(false);
        Destroy(obj.GetComponent<AdjustAscensionMenuItemsSpacing>());
        Destroy(obj.GetComponent<SequentialPixelTextLines>());
        Destroy(obj.transform.GetChild(4).gameObject);
        Destroy(obj.transform.GetChild(3).gameObject);
        Destroy(obj.transform.GetChild(2).gameObject);
        Destroy(obj.transform.GetChild(1).gameObject);

        Destroy(obj.GetComponent<ViewportRelativePosition>());
        ViewportRelativePosition positioning = obj.AddComponent<TextBehaviour>();
        positioning.viewportAnchor = new Vector2(0, 1);
        positioning.offset = new Vector2(0.05f, -0.12f);

        Transform textObj = obj.transform.GetChild(0);
        textObj.localPosition = Vector3.zero;

        Transform duplicatedTextObj = Instantiate(textObj, obj.transform);
        int layer = LayerMask.NameToLayer("OrthographicUI");
        duplicatedTextObj.gameObject.layer = layer;
        duplicatedTextObj.transform.GetChild(0).gameObject.layer = layer;
        duplicatedTextObj.localPosition += new Vector3(0, -0.05f, 1.05f);
        duplicatedTextObj.localScale *= 2;

        _textBehaviour = obj;
    }

    public static void SetActive(bool active)
    {
        Instance._textBehaviour.SetActive(active);
    }
}
