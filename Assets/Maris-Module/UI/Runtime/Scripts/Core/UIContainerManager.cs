using System.Reflection;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Sheets;

namespace GameModules.UI.Core
{
    public class UIContainerManager : MonoBehaviour
    {
        [SerializeField] private ScreenContainer screenContainer;
        [SerializeField] private ModalContainer popupContainer;
        [SerializeField] private SheetContainer sheetContainer;
        [SerializeField] private UnityScreenNavigatorSettings settings;

        public ScreenContainer ScreenContainer => screenContainer;
        public ModalContainer PopupContainer => popupContainer;
        public SheetContainer SheetContainer => sheetContainer;

        public void Initialize(UnityScreenNavigatorSettings settings){
            screenContainer.Settings = settings;
            popupContainer.Settings = settings;
            sheetContainer.Settings = settings;
            
            if (popupContainer != null && settings != null)
            {
                UpdateModalBackdropSetting(popupContainer, settings.DisableModalBackdrop);
            }
        }

        private void UpdateModalBackdropSetting(ModalContainer container, bool disableBackdrop)
        {
            if (container == null) return;
            
            var fieldInfo = typeof(ModalContainer).GetField("_disableBackdrop", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(container, disableBackdrop);
            }
        }

        private void Awake()
        {
            Initialize(settings);
        }
    }
}
