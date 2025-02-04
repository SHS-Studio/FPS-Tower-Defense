using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerManagerUI : MonoBehaviour
{
    [Header("Script Reference")]
    public GunInventory m_inventory;
    public PlayerMovementScript m_playerMovement;
    public MouseLookScript m_mouseLook;

    [Header("Button Options")]
    public GameObject uiPanel;
    public Button[] TowerOptions;
    public Button[] UltraUpgradeOptions;
    public Button[] UltimateUpgradeOptions;

    [Header("Button & Text Options")]
    public Button TowerSellOption;
    public Text TowerSelloptntxt;

    [Header("Tower Option")]
    public GameObject[] TowerPrefabs;
    public GameObject[] UltraTowerPrefabs;
    public GameObject[] UltimateTowerPrefabs;

    [Header("Coin System")]
    public int playerCoins = 1000; // Example coin balance for testing
    public int[] Costs;
    public int[] UltraCosts;
    public int[] UltimateCosts;

    [Header("Tower Information")]
    private GameObject currentTowerBase;
    private GameObject currentTower;
    private Vector3 spawnPoint;
   
    private Dictionary<GameObject, GameObject> towerAssignments = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        // Attach button listeners
        for (int i = 0; i < TowerOptions.Length; i++)
        {
            int index = i; // Capture index for closure
            TowerOptions[i].onClick.AddListener(() => SpawnTower(index));
        }

        for (int i = 0; i < UltraUpgradeOptions.Length; i++)
        {
            int index = i; // Capture index for closure
            UltraUpgradeOptions[i].onClick.AddListener(() => UpgradeToUltra(index));
        }

        for (int i = 0; i < UltimateUpgradeOptions.Length; i++)
        {
            int index = i; // Capture index for closure
            UltimateUpgradeOptions[i].onClick.AddListener(() => UpgradeToUltimate(index));
        }

        TowerSellOption.onClick.AddListener(SellTower);

        HideUI();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TowerBase"))
        {
            currentTowerBase = collision.gameObject;
            spawnPoint = currentTowerBase.transform.GetChild(0).position;

            if (towerAssignments.ContainsKey(currentTowerBase))
            {
                currentTower = towerAssignments[currentTowerBase];
                UpdateTowerUI();
            }
            else
            {
                ShowTowerUI();
            }
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void UpdateTowerUI()
    {
        if (currentTower == null) return;

        string towerTag = currentTower.tag;
        int towerIndex = GetCurrentTowerIndex();

        switch (towerTag)
        {
            case "Tower":
                ShowUpgradeUI(UltraUpgradeOptions, towerIndex);
                break;
            case "UltraTower":
                ShowUpgradeUI(UltimateUpgradeOptions, towerIndex);
                break;
            case "UltimateTower":
                ShowSellOnlyUI();
                break;
            default:
                Debug.LogWarning("Unexpected tower tag: " + towerTag);
                break;
        }
    }

    int GetCurrentTowerIndex()
    {
        if (currentTower == null) return -1;

        for (int i = 0; i < TowerPrefabs.Length; i++)
        {
            if (currentTower.name.Contains(TowerPrefabs[i].name)) return i;
        }
        for (int i = 0; i < UltraTowerPrefabs.Length; i++)
        {
            if (currentTower.name.Contains(UltraTowerPrefabs[i].name)) return i;
        }
        for (int i = 0; i < UltimateTowerPrefabs.Length; i++)
        {
            if (currentTower.name.Contains(UltimateTowerPrefabs[i].name)) return i;
        }
        return -1;
    }

    void ShowTowerUI()
    {
        uiPanel.SetActive(true);
        SetButtonVisibility(TowerOptions, true);
        SetButtonVisibility(UltraUpgradeOptions, false);
        SetButtonVisibility(UltimateUpgradeOptions, false);
        TowerSelloptntxt.text = "Tower Option".ToString();
        TowerSellOption.gameObject.SetActive(false);
        m_inventory.GetComponent<GunInventory>().enabled = false;
        m_mouseLook.GetComponent<MouseLookScript>().enabled = false;
    }

    void ShowUpgradeUI(Button[] upgradeOptions, int index)
    {
        uiPanel.SetActive(true);
        SetButtonVisibility(TowerOptions, false);
        SetButtonVisibility(UltraUpgradeOptions, false);
        SetButtonVisibility(UltimateUpgradeOptions, false);

        if (index >= 0 && index < upgradeOptions.Length)
        {
            upgradeOptions[index].gameObject.SetActive(true);
        }

        TowerSelloptntxt.text = "SELL".ToString();
        TowerSellOption.gameObject.SetActive(true);

        m_inventory.GetComponent<GunInventory>().enabled = false;
        m_mouseLook.GetComponent<MouseLookScript>().enabled = false;
    }

    void ShowSellOnlyUI()
    {
        uiPanel.SetActive(true);
        SetButtonVisibility(TowerOptions, false);
        SetButtonVisibility(UltraUpgradeOptions, false);
        SetButtonVisibility(UltimateUpgradeOptions, false);
        TowerSelloptntxt.text = "SELL".ToString();
        TowerSellOption.gameObject.SetActive(true);

        m_inventory.GetComponent<GunInventory>().enabled = false;
        m_mouseLook.GetComponent<MouseLookScript>().enabled = false;
    }

    void HideUI()
    {
        uiPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        m_inventory.GetComponent<GunInventory>().enabled = true;
        m_mouseLook.GetComponent<MouseLookScript>().enabled = true;
    }

    void SetButtonVisibility(Button[] buttons, bool isVisible)
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(isVisible);
        }
    }

    void SpawnTower(int index)
    {
        if (playerCoins >= Costs[index] && index >= 0 && index < TowerPrefabs.Length)
        {
            playerCoins -= Costs[index];
            currentTower = Instantiate(TowerPrefabs[index], spawnPoint, Quaternion.identity);
            currentTower.transform.parent = currentTowerBase.transform;
            currentTower.tag = "Tower";
            towerAssignments[currentTowerBase] = currentTower;
            HideUI();
        }
        else
        {
            Debug.Log("Not enough coins to spawn this tower!");
        }
    }

    void UpgradeToUltra(int index)
    {
        if (playerCoins >= UltraCosts[index] && index >= 0 && index < UltraTowerPrefabs.Length)
        {
            playerCoins -= UltraCosts[index];
            Destroy(currentTower);
            currentTower = Instantiate(UltraTowerPrefabs[index], spawnPoint, Quaternion.identity);
            currentTower.transform.parent = currentTowerBase.transform;
            currentTower.tag = "UltraTower";
            towerAssignments[currentTowerBase] = currentTower;
            HideUI();
        }
        else
        {
            Debug.Log("Not enough coins to upgrade to Ultra Tower!");
        }
    }

    void UpgradeToUltimate(int index)
    {
        if (playerCoins >= UltimateCosts[index] && index >= 0 && index < UltimateTowerPrefabs.Length)
        {
            playerCoins -= UltimateCosts[index];
            Destroy(currentTower);
            currentTower = Instantiate(UltimateTowerPrefabs[index], spawnPoint, Quaternion.identity);
            currentTower.transform.parent = currentTowerBase.transform;
            currentTower.tag = "UltimateTower";
            towerAssignments[currentTowerBase] = currentTower;
            HideUI();
        }
        else
        {
            Debug.Log("Not enough coins to upgrade to Ultimate Tower!");
        }
    }    
   
    void SellTower()
    {
        if (currentTower != null)
        {
            int refundAmount = 0;

            if (currentTower.CompareTag("Tower")) refundAmount = Costs[GetCurrentTowerIndex()] / 2;
            else if (currentTower.CompareTag("UltraTower")) refundAmount = UltraCosts[GetCurrentTowerIndex()] / 2;
            else if (currentTower.CompareTag("UltimateTower")) refundAmount = UltimateCosts[GetCurrentTowerIndex()] / 2;

            playerCoins += refundAmount;
            Destroy(currentTower);
            towerAssignments.Remove(currentTowerBase);
            currentTower = null;
            HideUI();
        }
    }

    public void CloseTowerUIPanel()
    {
        HideUI();
       
    }


    [Tooltip("HUD Coin  to display bullet count on screen. Will be find under name 'HUD_bullets' in scene.")]
    public TextMesh HUD_Coin;
    void OnGUI()
    {
        if (!HUD_Coin)
        {
            try
            {
                HUD_Coin = GameObject.Find("HUD_Coin").GetComponent<TextMesh>();
            }
            catch (System.Exception ex)
            {
                print("Couldnt find the HUD_Bullets ->" + ex.StackTrace.ToString());
            }
        }
        if (HUD_Coin)
            HUD_Coin.text = "COIN".ToString()  + " - " + playerCoins.ToString();

    
    }
}
