using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDeselectButtons : MonoBehaviour
{
    AudioSource audioSource;
    CategorySelect categorySelect;

    [Header("AudiClips")]
    [SerializeField] AudioClip SelectAllClip;
    [SerializeField] AudioClip DeselectAllClip;

    private void Awake()
    {
        categorySelect = GetComponentInParent<CategorySelect>();
        audioSource = GetComponent<AudioSource>();
    }
    public void SelectAllButton()
    {
        categorySelect.SelectAllCategories();
        audioSource.clip = SelectAllClip;
        audioSource.Play();
    }

    public void DeselectAllButton()
    {
        categorySelect.DeselectAllCategories();
        audioSource.clip = DeselectAllClip;
        audioSource.Play();
    }
}
