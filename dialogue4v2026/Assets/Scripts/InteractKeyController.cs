using UnityEngine;
using UnityEngine.UI;

public class InteractKeyController : MonoBehaviour
{
    private Vector3 interactPosition;
    private RectTransform rect;
    private Camera mainCamera;
    private Image interactImage;
    private bool isActive;

    private void OnEnable()
    {
        InteractOM.OnShowInteraction += ShowInteraction;
        InteractOM.OnPositionChange += GetInteractPosition;
    }

    private void OnDisable()
    {
        InteractOM.OnShowInteraction -= ShowInteraction;
        InteractOM.OnPositionChange -= GetInteractPosition;
    }


    private void Start()
    {
        rect = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        interactImage = GetComponent<Image>();
    }

    private void ShowInteraction(bool value)
    {
        isActive = value;
        interactImage.enabled = isActive;
        //interactImage.color = value ? Color.white : Color.clear;
    }

    private void GetInteractPosition(Vector3 value)
    {
        interactPosition = value;
    }

    private void Update()
    {
        if (!isActive) return;
        rect.position = mainCamera.WorldToScreenPoint(interactPosition);
    }
}
