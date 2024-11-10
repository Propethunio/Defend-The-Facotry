using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

public class GrabberUI : MonoBehaviour {

    public static GrabberUI Instance { get; private set; }



    [SerializeField] private List<ItemSO> itemSOList;

    private Dictionary<ItemSO, Transform> filterButtonDic;
    private Grabber grabber;

    private void Awake() {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Hide();
        };

        SetupFilter();

        Hide();
    }

    private void SetupFilter() {
        Transform filterContainer = transform.Find("FilterContainer");
        Transform filterTemplate = filterContainer.Find("Template");
        filterTemplate.gameObject.SetActive(false);

        // Destory old transforms
        foreach (Transform transform in filterContainer) {
            if (transform != filterTemplate) {
                Destroy(transform.gameObject);
            }
        }

        filterButtonDic = new Dictionary<ItemSO, Transform>();

        // Build transforms
        for (int i = 0; i < itemSOList.Count; i++) {
            ItemSO itemSO = itemSOList[i];
            Transform filterTransform = Instantiate(filterTemplate, filterContainer);
            filterTransform.gameObject.SetActive(true);

            filterButtonDic[itemSO] = filterTransform;

            filterTransform.Find("Icon").GetComponent<Image>().sprite = itemSO.sprite;

            filterTransform.GetComponent<Button_UI>().ClickFunc = () => {
                if (grabber != null) {
                    grabber.SetGrabFilterItemSO(itemSO);
                    UpdateFilter();
                }
            };
        }

        UpdateFilter();
    }

    private void UpdateFilter() {
        foreach (ItemSO itemSO in filterButtonDic.Keys) {
            if (grabber != null && grabber.GetGrabFilterItemSO() == itemSO) {
                // This one is selected
                filterButtonDic[itemSO].Find("Selected").gameObject.SetActive(true);
            } else {
                // Not selected
                filterButtonDic[itemSO].Find("Selected").gameObject.SetActive(false);
            }
        }
    }

    public void Show(Grabber grabber) {
        gameObject.SetActive(true);

        this.grabber = grabber;

        UpdateFilter();
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

}
