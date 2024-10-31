using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    [SerializeField] private CharacterMovement _selectedCharacter;

    public static GameManager Instance => _instance;

    public CharacterMovement SelectedCharacter => _selectedCharacter;

    private void Awake()
    {
        if (_instance == null) { _instance = this; }
    }
}
