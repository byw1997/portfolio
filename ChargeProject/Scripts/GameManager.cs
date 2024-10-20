using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.SceneManagement;
using NavMeshPlus.Components;
using System;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public PoolManager pool;
    public WeaponManager weaponmanager;
    public AudioMixer volumeMixer;
    public Spawner spawner;
    public Slider masterSlider;
    public Slider BGMSlider;
    public Slider SFXSlider;
    public int stage = 1;
    public MapGenerator mapgenerator;
    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject mainUI;
    public GameObject inPlayUI;
    public TileData tileData;
    public GameObject optionUI;
    public GameObject inventory;

    public bool paused;
    public bool invOn;

    public GameObject navMesh;
    public NavMeshSurface Surface2D;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(mainCamera);
        DontDestroyOnLoad(virtualCamera);
        DontDestroyOnLoad(inPlayUI);
        DontDestroyOnLoad(mainUI);
        DontDestroyOnLoad(navMesh);
        DontDestroyOnLoad(optionUI);
        DontDestroyOnLoad(inventory);
        paused = false;
        invOn = false;
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Stage"))
        {
            StartCoroutine(LoadNewStage());
        }
    }
    IEnumerator LoadNewStage()
    {
        GC.Collect();
        //mapgenerator.RemoveMap();
        mainUI.SetActive(false);
        player.gameObject.SetActive(false);
        SetTileData();
        yield return new WaitForSeconds(2f);
        yield return null;
        yield return StartCoroutine(mapgenerator.GenerateMap());
        //Surface2D = navMesh.GetComponent<NavMeshSurface>();
        Surface2D.BuildNavMesh();
        player.transform.position = new Vector3(0, 0, 0);
        player.gameObject.SetActive(true);
        inPlayUI.SetActive(true);
        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;
    }
    void SetTileData()
    {
        if(SceneManager.GetActiveScene().name == "Stage1")
        {
            mapgenerator.SetTileData(tileData.scene1, tileData.corridor1);
        }
        if(SceneManager.GetActiveScene().name == "Stage2")
        {
            mapgenerator.SetTileData(tileData.scene2, tileData.corridor2);
        }
        if(SceneManager.GetActiveScene().name == "Stage3")
        {
            mapgenerator.SetTileData(tileData.scene3, tileData.corridor3);
        }
        if(SceneManager.GetActiveScene().name == "Stage4")
        {
            mapgenerator.SetTileData(tileData.scene4, tileData.corridor4);
        }
        if(SceneManager.GetActiveScene().name == "Stage5")
        {
            mapgenerator.SetTileData(tileData.scene5, tileData.corridor5);
        }
        if(SceneManager.GetActiveScene().name == "Stage6")
        {
            mapgenerator.SetTileData(tileData.scene6, tileData.corridor6);
        }

    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("Stage"))
        {
            if (Input.GetButtonDown("Cancel") && !paused)
            {
                OpenOptionMenu();
            }
            else if(Input.GetButtonDown("Cancel") && paused && invOn)
            {
                CloseOptionMenu();
            }
            else if(Input.GetButtonDown("Cancel") && paused)
            {
                CloseOptionMenu();
            }
            if (Input.GetButtonDown("Inventory") && !paused)
            {
                OpenInventory();
            }
            else if(Input.GetButtonDown("Inventory") && paused)
            {
                CloseInventory();
            }
        }
    }
    public void MasterControl()
    {
        AudioControl(masterSlider);
    }

    public void BGMControl()
    {
        AudioControl(BGMSlider);
    }

    public void SFXControl()
    {
        AudioControl(SFXSlider);
    }

    public void AudioControl(Slider s)
    {
        float sound = s.value;

        if(sound == -40f)
        {
            volumeMixer.SetFloat(s.name, -80);
        }
        else
        {
            volumeMixer.SetFloat(s.name, sound);
        }
    }

    public void ToggleWholeAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        paused = true;
    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
    }

    public void OpenOptionMenu()
    {
        Pause();
        optionUI.SetActive(true);
    }
    public void CloseOptionMenu()
    {
        Resume();
        optionUI.SetActive(false);
    }
    public void OpenInventory()
    {
        Pause();
        inventory.SetActive(true);
    }
    public void CloseInventory()
    {
        Resume();
        inventory.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
