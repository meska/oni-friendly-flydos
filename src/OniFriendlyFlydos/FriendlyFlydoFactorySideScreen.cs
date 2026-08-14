using System.Globalization;
using PeterHan.PLib.UI;
using TMPro;
using UnityEngine;

namespace OniFriendlyFlydos
{
    public sealed class FriendlyFlydoFactorySideScreen : SideScreenContent, ISim1000ms
    {
        private GameObject enabledCheckbox;
        private GameObject avoidWaterCheckbox;
        private GameObject targetField;
        private LocText statusLabel;
        private FriendlyFlydoFactoryController controller;
        private bool refreshing;
        private bool invalidTarget;

        public override string GetTitle()
        {
            return FriendlyFlydosStrings.UI.FactorySideScreen.Title;
        }

        public override bool IsValidForTarget(GameObject target)
        {
            return target != null
                && target.GetComponent<FriendlyFlydoFactoryController>() != null;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            var panel = new PPanel("FriendlyFlydoFactoryPanel")
            {
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.UpperLeft,
                FlexSize = new Vector2(1f, 0f),
                Margin = new RectOffset(8, 8, 8, 8),
                Spacing = 8
            };
            var maintainMinimum = new PCheckBox("MaintainMinimum")
            {
                Text = FriendlyFlydosStrings.UI.FactorySideScreen.Participate,
                ToolTip = FriendlyFlydosStrings.UI.FactorySideScreen.ParticipateTooltip,
                CheckSize = new Vector2(20f, 20f),
                FlexSize = new Vector2(1f, 0f),
                TextAlignment = TextAnchor.MiddleLeft,
                OnChecked = OnEnabledChanged
            }.SetKleiPinkStyle();
            // El fondo del pannello xe ciaro: dopo el preset rosa va ripristinato el testo scuro.
            maintainMinimum.TextStyle = PUITuning.Fonts.UIDarkStyle;
            panel.AddChild(maintainMinimum.AddOnRealize(realized => enabledCheckbox = realized));
            panel.AddChild(new PLabel("TargetLabel")
            {
                Text = FriendlyFlydosStrings.UI.FactorySideScreen.Target,
                ToolTip = FriendlyFlydosStrings.UI.FactorySideScreen.TargetTooltip,
                TextStyle = PUITuning.Fonts.UIDarkStyle,
                TextAlignment = TextAnchor.MiddleLeft,
                FlexSize = new Vector2(1f, 0f)
            });
            panel.AddChild(
                new PTextField("TargetCount")
                {
                    Type = PTextField.FieldType.Integer,
                    MaxLength = 2,
                    Text = "5",
                    ToolTip = FriendlyFlydosStrings.UI.FactorySideScreen.TargetInputTooltip,
                    OnTextChanged = OnTargetChanged
                }.SetKleiBlueStyle()
                    .SetMinWidthInCharacters(4)
                    .AddOnRealize(realized => targetField = realized));
            var avoidWater = new PCheckBox("AvoidWater")
            {
                Text = FriendlyFlydosStrings.UI.FactorySideScreen.AvoidWater,
                ToolTip = FriendlyFlydosStrings.UI.FactorySideScreen.AvoidWaterTooltip,
                CheckSize = new Vector2(20f, 20f),
                FlexSize = new Vector2(1f, 0f),
                TextAlignment = TextAnchor.MiddleLeft,
                OnChecked = OnAvoidWaterChanged
            }.SetKleiPinkStyle();
            avoidWater.TextStyle = PUITuning.Fonts.UIDarkStyle;
            panel.AddChild(avoidWater.AddOnRealize(realized => avoidWaterCheckbox = realized));
            panel.AddChild(new PLabel("Status")
            {
                Text = string.Format(
                    FriendlyFlydosStrings.UI.FactorySideScreen.Status,
                    0,
                    0,
                    0,
                    0,
                    0),
                TextStyle = PUITuning.Fonts.UIDarkStyle,
                TextAlignment = TextAnchor.UpperLeft,
                FlexSize = new Vector2(1f, 0f)
            }.AddOnRealize(realized => statusLabel = realized.GetComponentInChildren<LocText>()));

            ContentContainer = panel.AddTo(gameObject, 0);
            Refresh();
        }

        public override void SetTarget(GameObject target)
        {
            controller = IsValidForTarget(target)
                ? target.GetComponent<FriendlyFlydoFactoryController>()
                : null;
            Refresh();
        }

        public override void ClearTarget()
        {
            controller = null;
            invalidTarget = false;
            base.ClearTarget();
        }

        public void Sim1000ms(float dt)
        {
            // El pannello resta vivo mentre i Flydo nasce o more.
            RefreshStatus();
        }

        private void OnEnabledChanged(GameObject _, int checkState)
        {
            if (!refreshing)
            {
                controller?.SetEnabled(CheckboxPolicy.GetValueAfterClick(checkState));
                Refresh();
            }
        }

        private void OnTargetChanged(GameObject _, string text)
        {
            if (refreshing || controller == null)
            {
                return;
            }

            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                controller.SetTargetCount(value);
                invalidTarget = false;
                targetField.GetComponent<TMP_InputField>()?.SetTextWithoutNotify(
                    controller.TargetCount.ToString(CultureInfo.InvariantCulture));
                RefreshStatus();
            }
            else
            {
                invalidTarget = true;
                RefreshStatus();
            }
        }

        private void OnAvoidWaterChanged(GameObject _, int checkState)
        {
            if (!refreshing)
            {
                controller?.SetAvoidWater(CheckboxPolicy.GetValueAfterClick(checkState));
                Refresh();
            }
        }

        private void Refresh()
        {
            if (controller == null
                || enabledCheckbox == null
                || avoidWaterCheckbox == null
                || targetField == null)
            {
                return;
            }

            refreshing = true;
            PCheckBox.SetCheckState(
                enabledCheckbox,
                controller.Enabled
                    ? PCheckBox.STATE_CHECKED
                    : PCheckBox.STATE_UNCHECKED);
            PCheckBox.SetCheckState(
                avoidWaterCheckbox,
                controller.AvoidWater
                    ? PCheckBox.STATE_CHECKED
                    : PCheckBox.STATE_UNCHECKED);
            targetField.GetComponent<TMP_InputField>()?.SetTextWithoutNotify(
                controller.TargetCount.ToString(CultureInfo.InvariantCulture));
            refreshing = false;
            SetTargetFieldEnabled();
            RefreshStatus();
        }

        private void SetTargetFieldEnabled()
        {
            var input = targetField?.GetComponent<TMP_InputField>();
            if (input != null)
            {
                input.interactable = controller != null && controller.Enabled;
            }
        }

        private void RefreshStatus()
        {
            if (controller != null && statusLabel != null)
            {
                if (invalidTarget)
                {
                    statusLabel.SetText(FriendlyFlydosStrings.UI.FactorySideScreen.InvalidTarget);
                    return;
                }

                var snapshot = controller.GetProductionSnapshot();
                statusLabel.SetText(string.Format(
                    FriendlyFlydosStrings.UI.FactorySideScreen.Status,
                    snapshot.Living,
                    snapshot.EffectiveTarget,
                    snapshot.QueuedGlobally,
                    snapshot.QueuedHere,
                    snapshot.ParticipatingStations));
            }
        }
    }
}
