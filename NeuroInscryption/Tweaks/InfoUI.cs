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

public sealed class InfoUI : Singlyboi<InfoUI>
{
    public sealed class TextBehaviour : ViewportRelativePosition
    {
        private static string Text
        {
            get
            {
#if !DEBUG
                // This text only displays during manual input anyway
                return $"MANUAL INPUT ENABLED - Current flow: <color=#eef4c6>{AutomationManager.CurrentFlowName}</color>";
#else
                return $"Current flow: <color=#eef4c6>{AutomationManager.FlowRunner.FlowName}</color>{(!AutomationManager.FlowRunner.AllowManualInterruption ? " (manual input blocked)" : "")}{(AutomationManager.AllowManualInput ? " - MANUAL INPUT ENABLED" : "")}";
#endif
            }
        }

        private float _swapTimer = 0.5f;
        public PixelText[] texts = [];

        private new void Awake()
        {
            UpdateCamera();
            base.Awake();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            foreach (PixelText text in texts)
            {
                text.SetText(Text);
            }

            foreach (PixelText text in texts)
            {
                text.gameObject.SetActive(true);
            }
        }

        public override void ManagedUpdate()
        {
            UpdateCamera();
            base.ManagedUpdate();

            foreach (PixelText text in texts)
            {
                text.SetText(Text);
            }

            _swapTimer -= Time.deltaTime;

            if (_swapTimer <= 0)
            {
                _swapTimer = 0.5f;
#if !DEBUG
                foreach (PixelText text in texts)
                {
                    text.gameObject.SetActive(!text.gameObject.activeSelf);
                }
#endif
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

    [UsedImplicitly]
    private static GameObject _textBehaviour = null!;

    // ReSharper disable once Unity.IncorrectMethodSignature
    [UsedImplicitly]
    private async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == "Ascension_Configure");

        SequentialPixelTextLines foundText = UnityEngine.Resources.FindObjectsOfTypeAll<SequentialPixelTextLines>()
            .First(t => t.name == "Footer" && t.transform.parent.name == "ChallengesScreen");

        GameObject obj = Instantiate(foundText.transform.gameObject, transform);
#if !DEBUG
        obj.SetActive(false);
#endif
        Destroy(obj.GetComponent<AdjustAscensionMenuItemsSpacing>());
        Destroy(obj.GetComponent<SequentialPixelTextLines>());
        Destroy(obj.transform.GetChild(1).gameObject);
        Destroy(obj.transform.GetChild(2).gameObject);
        Destroy(obj.transform.GetChild(3).gameObject);
        Destroy(obj.transform.GetChild(4).gameObject);

        Destroy(obj.GetComponent<ViewportRelativePosition>());

        Transform textObj = obj.transform.GetChild(0);
        textObj.localPosition = Vector3.zero;

        Transform duplicatedTextObj = Instantiate(textObj, obj.transform);
        int layer = LayerMask.NameToLayer("OrthographicUI");
        duplicatedTextObj.gameObject.layer = layer;
        duplicatedTextObj.transform.GetChild(0).gameObject.layer = layer;
        duplicatedTextObj.localPosition += new Vector3(0, -0.05f, 1.05f);
        duplicatedTextObj.localScale *= 2;

        TextBehaviour customTextBehaviour = obj.AddComponent<TextBehaviour>();
        customTextBehaviour.viewportAnchor = new Vector2(0, 1);
        customTextBehaviour.offset = new Vector2(0.05f, -0.12f);
        customTextBehaviour.texts = [textObj.GetComponent<PixelText>(), duplicatedTextObj.GetComponent<PixelText>()];
        _textBehaviour = obj;
    }

    public static void SetManualInputActive(bool active)
    {
#if !DEBUG
        _textBehaviour.SetActive(active);
#endif
    }
}
