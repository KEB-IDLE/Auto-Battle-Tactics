using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private GameObject championGO;
    private ChampionController championController;
    public Image fillImage;

    [Header("Colors")]
    [SerializeField] private Color playerColor = new Color(0.21f, 0.82f, 0.36f, 1f); // 초록
    [SerializeField] private Color enemyColor  = new Color(0.93f, 0.20f, 0.20f, 1f); // 빨강

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (championGO != null)
        {
            this.transform.position = championGO.transform.position
                                      + new Vector3(0, 1.5f + 1.5f * championGO.transform.localScale.x, 0);

            fillImage.fillAmount = championController.currentHealth / championController.maxHealth;
            canvasGroup.alpha = (championController.currentHealth <= 0) ? 0 : 1;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Init(GameObject _championGO)
    {
        championGO = _championGO;
        championController = championGO.GetComponent<ChampionController>();

        // ✅ 적(TEAMID_AI)이면 빨강, 아니면 초록
        if (fillImage && championController)
        {
            bool isEnemy = (championController.teamID == ChampionController.TEAMID_AI);
            fillImage.color = isEnemy ? enemyColor : playerColor;
        }
    }
}
