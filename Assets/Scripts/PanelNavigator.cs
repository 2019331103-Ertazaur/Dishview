using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelNavigator : MonoBehaviour
{
    [SerializeField]
    private List<Panel> panels = new List<Panel>();
    [SerializeField]
    public bool canGoBack = true;
    public int defaultSelected;
    private bool initialNavigation = false;
    public int CurrentPanelIndex { get; private set; }
    public int NextPanelIndex { get; private set; }
    public Panel CurrentPanel  { get; private set; }
    public Panel NextPanel { get; private set; }
    public List<int> lastPanelsIndexs { get; private set; } = new List<int>();

    private static List<PanelNavigator> instances = new List<PanelNavigator>();
    private static bool Paused;
    private bool dontAddToLast;



    private void OnEnable()
    {

        lastPanelsIndexs.Clear();
        if(defaultSelected >= 0)
        {
            initialNavigation = true;
            GoToView(defaultSelected);
        }
        if (!instances.Contains(this))
        {
            instances.Add(this);
        }
    }

    private void OnDisable()
    {
        if (instances.Contains(this))
        {
            instances.Remove(this);
        }
    }

    private void OnDestroy()
    {
        if (instances.Contains(this))
        {
            instances.Remove(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame && !Paused)
        {
            var instance = instances.LastOrDefault(i => i.canGoBack);
            if (instance) instance.GoToLastView();
        }
    }

    public void SetDefault(int index)
    {
        defaultSelected = index;
    }
    public void GoToLastView()
    {
        if (!canGoBack) return;
        if (lastPanelsIndexs.Count < 1) return;
        int index = lastPanelsIndexs.Last();
        if (index == CurrentPanelIndex) return;
        Panel lastPanel = panels[CurrentPanelIndex];
        GoToView(index);
        lastPanel.Backed();
    }

    public void GoToView(int index)
    {
        StartCoroutine(GoToViewInternal(index));
    }
    
    public void GoToViewAndForget(int index)
    {
        dontAddToLast = true;
        StartCoroutine(GoToViewInternal(index));
    }

    public void GoToViewAndClearPrevious(int index)
    {
        StartCoroutine(GoToViewInternal(index, -1));
    }
    public void GoToViewAndClearPreviousCount(int index, int count)
    {
        StartCoroutine(GoToViewInternal(index, count));
    }

    private IEnumerator GoToViewInternal(int index, int count = -2)
    {
        if (index >= 0 && index < panels.Count)
        {
            if(!initialNavigation)
            {
                CurrentPanel = panels[CurrentPanelIndex];

                if (CurrentPanel)
                {
                    yield return CurrentPanel.HideAsync();
                }
            }
            else
            {
                initialNavigation = false;
            }
            NextPanelIndex = index;
            NextPanel = panels[NextPanelIndex];
            NextPanel.gameObject.SetActive(true);
            yield return NextPanel.ShowAsync();
            if (lastPanelsIndexs.Count > 0)
            {
                if (NextPanelIndex == 0 || count != -2)
                {
                    if(count == -1)
                    {
                        lastPanelsIndexs.Clear();
                    }
                    else
                    {
                        lastPanelsIndexs = lastPanelsIndexs.SkipLast(count).ToList();
                    }
                }
                else
                {
                    if (NextPanelIndex == lastPanelsIndexs.Last())
                    {
                        lastPanelsIndexs.Remove(NextPanelIndex);
                    }
                    else if (!dontAddToLast)
                    {
                        lastPanelsIndexs.Add(CurrentPanelIndex);
                    }
                }
            }
            else
            {
                if(count == -2 && CurrentPanelIndex != NextPanelIndex && !dontAddToLast)
                    lastPanelsIndexs.Add(CurrentPanelIndex);
            }
            CurrentPanelIndex = NextPanelIndex;
            dontAddToLast = false;
        }
        else yield break;
    }

    public static void Pause()
    {
        if (Paused) return;
        Paused = true;
    }
    public static void Resume()
    {
        if (!Paused) return;
        Paused = false;
    }
}
