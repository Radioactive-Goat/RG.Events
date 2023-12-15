using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RG.Events
{
    public class EventDebugger : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset = default;

        private Label _statusLabel = null;
        private ListView _eventList = null;
        private ListView _subscriberList = null;
        private ScrollView _invokeStack = null;
        private UnsignedIntegerField _bufferSize = null;

        private EventCallback<ChangeEvent<uint>> _onBufferSizeChanged = (change) => { EventSystem.Instance.invokeStackBufferSize = change.newValue; };

        [MenuItem("Radioactive Goat/Event System/Event Debugger")]
        public static void ShowEventDebuggerWindow()
        {
            EventDebugger wnd = GetWindow<EventDebugger>();
            wnd.titleContent = new GUIContent("Event Debugger");
        }

        private void OnPlayModeStateChange(PlayModeStateChange stateChange)
        {
            CheckStatus();

            if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                _bufferSize.value = 0;
                _bufferSize.UnregisterValueChangedCallback(_onBufferSizeChanged);
                ClearAllLists();
                _invokeStack.Clear();
                return;
            }

            if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                _bufferSize.value = EventSystem.Instance.invokeStackBufferSize;
                _bufferSize.RegisterValueChangedCallback(_onBufferSizeChanged);
                BuildAllLists();
            }

        }

        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            ClearAllLists();
            _invokeStack.Clear();
        }

        private void OnInspectorUpdate()
        {
            if (EditorApplication.isPlaying)
            {
                BuildAllLists();
                _eventList.Rebuild();
                _subscriberList.Rebuild();
            }
        }

        public void CreateGUI()
        {
            _visualTreeAsset.CloneTree(rootVisualElement);

            _statusLabel = rootVisualElement.Q<Label>("Status");
            _eventList = rootVisualElement.Q<ListView>("EventList");
            _subscriberList = rootVisualElement.Q<ListView>("SubscriberList");
            _invokeStack = rootVisualElement.Q<ScrollView>("InvokeList");
            _bufferSize = rootVisualElement.Q<UnsignedIntegerField>("BufferSize");

            EditorApplication.playModeStateChanged += OnPlayModeStateChange;

            if (!CheckStatus())
            {
                return;
            }

            _bufferSize.value = EventSystem.Instance.invokeStackBufferSize;

            BuildInvokeStack();

            EventSystem.Instance.invokeEvent.AddListener(UpdateInvokeStack);
        }

        private Label BuildListLabel()
        {
            Label label = new Label();

            label.style.unityTextAlign = TextAnchor.MiddleLeft;

            return label;
        }

        private void BuildAllLists()
        {
            _eventList.itemsSource = EventSystem.Instance.Events.ToList();
            _eventList.makeItem = BuildListLabel;
            _eventList.bindItem = (e, i) => (e as Label).text = EventSystem.Instance.Events.ToList()[i].Key.FullName;
            _eventList.selectionType = SelectionType.Single;

            if (_eventList.selectedIndex < EventSystem.Instance.Events.ToList().Count && _eventList.selectedIndex >= 0)
            {
                _subscriberList.itemsSource = EventSystem.Instance.Events.ToList()[_eventList.selectedIndex].Value.Callbacks;
                _subscriberList.makeItem = BuildListLabel;
                _subscriberList.bindItem = (e, i) => (e as Label).text = EventSystem.Instance.Events.ToList()[_eventList.selectedIndex].Value.Callbacks[i];
                _subscriberList.selectionType = SelectionType.None;
            }
        }

        private void ClearAllLists()
        {
            _eventList.itemsSource = null;
            _eventList.Rebuild();
            _subscriberList.itemsSource = null;
            _subscriberList.Rebuild();
        }

        private void BuildInvokeStack()
        {
            _invokeStack.Clear();
            foreach (var item in EventSystem.Instance.invokeStack)
            {
                var foldout = new Foldout();
                foldout.text = $"[{item.TimeStamp}] {item.EventName}";
                foreach (var data in item.ArgumentData.GetType().GetFields())
                {
                    var tf = new TextField(data.Name);
                    tf.value = data.GetValue(item.ArgumentData).ToString();
                    tf.isReadOnly = true;
                    foldout.Add(tf);
                }
                foldout.value = false;
                _invokeStack.Add(foldout);
            }
        }

        private void UpdateInvokeStack(InvokationMetaData arg)
        {
            var foldout = new Foldout();
            foldout.text = $"[{arg.TimeStamp}] {arg.EventName}";
            foreach (var data in arg.ArgumentData.GetType().GetFields())
            {
                var tf = new TextField(data.Name);
                tf.value = data.GetValue(arg.ArgumentData).ToString();
                tf.isReadOnly = true;
                foldout.Add(tf);
            }
            foldout.value = false;
            _invokeStack.Add(foldout);
        }

        private bool CheckStatus()
        {
            if (!EditorApplication.isPlaying)
            {
                _statusLabel.text = "Game is not running!";
                _statusLabel.style.color = Color.red;
                return false;
            }

            if (EventSystem.Instance == null)
            {
                _statusLabel.text = "Event System not initialized!";
                _statusLabel.style.color = Color.red;
                return false;
            }

            _statusLabel.text = "All OK!";
            _statusLabel.style.color = Color.green;
            return true;
        }
    }
}
