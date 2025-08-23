using UnityEngine;

public class PrefabIcon : MonoBehaviour
{
    public Sprite icon;

    // 카드 이미지를 컨테이너에 '꽉 채우기(cover)'로 표시할지
    public bool cover = true;

    // 필요하면 수동 비율(0이면 스프라이트 비율 사용)
    public float aspectOverride = 0f;
}
