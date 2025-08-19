/// <summary>
/// dto.cs 파일에는 클라이언트와 서버 간 데이터 교환을 위한 모든 DTO(Data Transfer Object)가 정의되어 있습니다.
/// 이 파일의 DTO들은 모두 [System.Serializable]을 사용하여 Unity에서 직렬화가 가능하며,
/// 서버와 클라이언트 간 HTTP 요청/응답 혹은 JSON 데이터 변환에 활용됩니다.
/// </summary>
/// 

/// LoginController
[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public string token;
}

[System.Serializable]
public class RegisterRequest
{
    public string email;
    public string password;
    public string nickname;
}

[System.Serializable]
public class RegisterResponse
{
    public bool success;
    public string message;
}

/// UIManager
[System.Serializable]
class ProfileIconUpdateRequest
{
    public int profile_icon_id;
}

[System.Serializable]
class ProfileCharacterUpdateRequest
{
    public int profile_char_id;
}

[System.Serializable]
class LevelUpdateRequest
{
    public int level;
}

[System.Serializable]
class ExpUpdateRequest
{
    public int exp;
}

[System.Serializable]
class GoldUpdateRequest
{
    public int gold;
}

[System.Serializable]
public class UserRecordUpdateRequest
{
    public int rank_match_count;
    public int rank_wins;
    public int rank_losses;
    public int rank_point;
}

[System.Serializable]
public class UserProfileResponse
{
    public string message;
    public bool success;
    public UserProfile data;
}

[System.Serializable]
public class UserRecordResponse
{
    public bool success;
    public string message;
    public UserRecord data;
}

[System.Serializable]
public class UserProfile
{
    public int user_id;
    public string nickname;
    public int profile_icon_id;
    public int profile_char_id;
    public int level;
    public int exp;
    public int gold;
}

[System.Serializable]
public class UserRecord
{
    public int user_id;
    public string last_login_at;
    public int rank_match_count;
    public int rank_wins;
    public int rank_losses;
    public int rank_point;
    public string tier;
    public int global_rank;
}

[System.Serializable]
public class IconPurchaseRequest
{
    public int icon_id;
}

[System.Serializable]
public class IconPurchaseResponse
{
    public bool success;
    public string message;
    public int gold;
}

[System.Serializable]
public class GlobalRankEntry
{
    public int rank;
    public string nickname;
    public int profile_icon_id;
    public int rank_point;
}

[System.Serializable]
public class GlobalRankingResponse
{
    public bool success;
    public GlobalRankEntry[] data;
}
