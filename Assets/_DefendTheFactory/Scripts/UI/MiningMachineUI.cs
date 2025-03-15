using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeMonkey.Utils;

public class MiningMachineUI : MonoBehaviour {

    public static MiningMachineUI Instance { get; private set; }


    private GatheringMachine miningMachine;
    private TextMeshProUGUI miningItemText;

    private void Awake() {
        Instance = this;

        transform.Find("CloseBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Hide();
        };

        miningItemText = transform.Find("MiningItem").Find("Text").GetComponent<TextMeshProUGUI>();

        Hide();
    }

    private void MiningMachine_OnItemStorageCountChanged(object sender, System.EventArgs e) {
        UpdateText();
    }

    private void UpdateText() {
        miningItemText.text = miningMachine.GetItemStoredCount(GameAssets.i.itemSO_Refs.any).ToString();
    }


    public void Show(GatheringMachine miningMachine) {
        gameObject.SetActive(true);

        if (this.miningMachine != null) {
            this.miningMachine.OnItemStorageCountChanged -= MiningMachine_OnItemStorageCountChanged;
        }

        this.miningMachine = miningMachine;

        if (miningMachine != null) {
            transform.Find("MiningItem").Find("Icon").GetComponent<Image>().sprite = miningMachine.GetMiningResourceItem()?.sprite;

            miningMachine.OnItemStorageCountChanged += MiningMachine_OnItemStorageCountChanged;
        }

        UpdateText();
    }

    public void Hide() {
        gameObject.SetActive(false);

        if (this.miningMachine != null) {
            this.miningMachine.OnItemStorageCountChanged -= MiningMachine_OnItemStorageCountChanged;
        }

        miningMachine = null;
    }

}
