using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Updates and controls UI elements
/// </summary>
public class UIController : MonoBehaviour
{
    [SerializeField, Range(0.5f, 1f)]
    float imageScale = 0.9f;
    public ChampionShop championShop;
    public GamePlayController gamePlayController;

    public GameObject[] championsFrameArray;
    public GameObject[] bonusPanels;


    public Text timerText;
    public Text championCountText;
    public Text goldText;
    public Text hpText;

    public GameObject shop;
    public GameObject restartButton;
    public GameObject placementText;
    public GameObject gold;
    public GameObject bonusContainer;
    public GameObject bonusUIPrefab;


    /// <summary>
    /// Called when a chamipon panel clicked on shop UI
    /// </summary>
    public void OnChampionClicked()
    {
        //get clicked champion ui name
        string name = EventSystem.current.currentSelectedGameObject.transform.parent.name;

        //calculate index from name
        string defaultName = "champion container ";
        int championFrameIndex = int.Parse(name.Substring(defaultName.Length, 1));

        //message shop from click
        championShop.OnChampionFrameClicked(championFrameIndex);
    }

    /// <summary>
    /// Called when refresh button clicked on shop UI
    /// </summary>
    public void Refresh_Click()
    {
        championShop.RefreshShop(false);
    }

    /// <summary>
    /// Called when buyXP button clicked on shop UI
    /// </summary>
    public void BuyXP_Click()
    {
        championShop.BuyLvl();
    }

    /// <summary>
    /// Called when restart button clicked on UI
    /// </summary>
    public void Restart_Click()
    {
        gamePlayController.RestartGame();
    }

    /// <summary>
    /// hides chamipon ui frame
    /// </summary>
    public void HideChampionFrame(int index)
    {
        championsFrameArray[index].transform.Find("champion").gameObject.SetActive(false);
    }

    /// <summary>
    /// make shop items visible
    /// </summary>
    public void ShowShopItems()
    {
        //unhide all champion frames
        for (int i = 0; i < championsFrameArray.Length; i++)
        {
            championsFrameArray[i].transform.Find("champion").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// displays champion info to given index on UI
    /// </summary>
    /// <param name="champion"></param>
    /// <param name="index"></param>
    public void LoadShopItem(Champion champion, int index)
    {
        // 루트
        Transform championUI = championsFrameArray[index].transform.Find("champion");
        if (!championUI) { Debug.LogError($"[UI] 'champion' not found at slot {index}"); return; }

        // 새 구조: champion 바로 아래 자식들만 사용
        var img = championUI.Find("Image")?.GetComponent<Image>(); // 큰 이미지
        var nameT = championUI.Find("Name")?.GetComponent<Text>();
        var costT = championUI.Find("Cost")?.GetComponent<Text>();
        var typeI = championUI.Find("TypeIcon")?.GetComponent<Image>(); // 타입은 아이콘만
                                                                        // coin(우상 코인)은 에디터에서 스프라이트만 넣어두면 코드 불필요

        // 큰 이미지: 프리팹의 PrefabIcon.icon
        if (img)
        {
            var s = (champion && champion.prefab)
                    ? champion.prefab.GetComponentInChildren<PrefabIcon>(true)?.icon
                    : null;

            img.sprite = s;
            img.type = Image.Type.Simple;
            img.preserveAspect = true;

            var slot = championUI.GetComponent<RectTransform>();   // 부모(슬롯)
            var rt = img.rectTransform;

            // 1) 이미지 Rect를 중앙 고정
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;

            // 2) 실제 슬롯 크기 기준으로 “가로 n%” 사이즈 계산 (비율 유지)
            Canvas.ForceUpdateCanvases();
            float slotW = slot.rect.width;
            float slotH = slot.rect.height;

            // 스프라이트 비율(없으면 2:3 가정)
            float aspect = (s && s.rect.height > 0) ? (s.rect.width / s.rect.height) : (2f / 3f);

            // 우선 가로 기준으로 축소
            float targetW = slotW * imageScale;
            float targetH = targetW / aspect;

            // 세로가 슬롯을 넘치면 세로 기준으로 재조정
            if (targetH > slotH * imageScale)
            {
                targetH = slotH * imageScale;
                targetW = targetH * aspect;
            }

            // 3) 최종 사이즈 적용
            rt.sizeDelta = new Vector2(targetW, targetH);

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // cover 강제하던 ARF 제거
            var arf = img.GetComponent<AspectRatioFitter>();
            if (arf) Destroy(arf);
        }
        var coinRoot = championUI.Find("coin");
        var coinText = coinRoot ? coinRoot.GetComponentInChildren<Text>(true) : null;
        if (coinText) coinText.text = champion ? champion.cost.ToString() : "";

        // 텍스트
        if (nameT) nameT.text = champion ? champion.uiname : "-";
        if (costT) costT.text = champion ? champion.cost.ToString() : "0";

        // 타입 아이콘(이름 텍스트는 표시하지 않음)
        if (typeI)
        {
            var spr = (champion && champion.type != null) ? champion.type.icon : null;
            typeI.sprite = spr;
            typeI.enabled = (spr != null);
        }
    }

    /// <summary>
    /// Updates ui when needed
    /// </summary>
    public void UpdateUI()
    {
        goldText.text = gamePlayController.currentGold.ToString();
        championCountText.text = gamePlayController.currentChampionCount.ToString() + " / " + gamePlayController.currentChampionLimit.ToString();
        hpText.text = gamePlayController.currentHP.ToString();


        //hide bonusus UI
        foreach (GameObject go in bonusPanels)
        {
            go.SetActive(false);
        }


        //if not null
        if (gamePlayController.championTypeCount != null)
        {
            int i = 0;
            //iterate bonuses
            foreach (KeyValuePair<ChampionType, int> m in gamePlayController.championTypeCount)
            {
                //Now you can access the key and value both separately from this attachStat as:
                GameObject bonusUI = bonusPanels[i];
                bonusUI.transform.SetParent(bonusContainer.transform);
                bonusUI.transform.Find("icon").GetComponent<Image>().sprite = m.Key.icon;
                bonusUI.transform.Find("name").GetComponent<Text>().text = m.Key.displayName;
                bonusUI.transform.Find("count").GetComponent<Text>().text = m.Value.ToString() + " / " + m.Key.championBonus.championCount.ToString();

                bonusUI.SetActive(true);

                i++;
            }
        }
    }

    /// <summary>
    /// updates timer
    /// </summary>
    public void UpdateTimerText()
    {
        timerText.text = gamePlayController.timerDisplay.ToString();
    }

    /// <summary>
    /// sets timer visibility
    /// </summary>
    /// <param name="b"></param>
    public void SetTimerTextActive(bool b)
    {
        timerText.gameObject.SetActive(b);

        placementText.SetActive(b);
    }

    /// <summary>
    /// displays loss screen when game ended
    /// </summary>
    public void ShowLossScreen()
    {
        SetTimerTextActive(false);
        shop.SetActive(false);
        gold.SetActive(false);


        restartButton.SetActive(true);
    }

    /// <summary>
    /// displays game screen when game started
    /// </summary>
    public void ShowGameScreen()
    {
        SetTimerTextActive(true);
        shop.SetActive(true);
        gold.SetActive(true);


        restartButton.SetActive(false);
    }

}