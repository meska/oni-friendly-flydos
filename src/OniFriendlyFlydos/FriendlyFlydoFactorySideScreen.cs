using System.Globalization;
using PeterHan.PLib.UI;
using TMPro;
using UnityEngine;

namespace OniFriendlyFlydos
{
    public sealed class FriendlyFlydoFactorySideScreen : SideScreenContent
    {
        private GameObject enabledCheckbox;
        private GameObject targetField;
        private LocText statusLabel;
        private FriendlyFlydoFactoryController controller;
        private bool refreshing;

        public override string GetTitle()
        {
            return "Friendly Flydo Production";
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
                Margin = new RectOffset(8, 8, 8, 8),
                Spacing = 8
            };
            panel.AddChild(
                new PCheckBox("MaintainMinimum")
                {
                    Text = "Maintain a colony minimum",
                    ToolTip = "This station automatically keeps enough Flydo orders queued to maintain the target across the colony.",
                    CheckSize = new Vector2(20f, 20f),
                    OnChecked = OnEnabledChanged
                }.SetKleiPinkStyle().AddOnRealize(realized => enabledCheckbox = realized));
            panel.AddChild(new PLabel("TargetLabel")
            {
                Text = "Target active Flydos",
                ToolTip = "The highest enabled target in this world is shared by all participating stations."
            });
            panel.AddChild(
                new PTextField("TargetCount")
                {
                    Type = PTextField.FieldType.Integer,
                    MaxLength = 2,
                    Text = "5",
                    ToolTip = "Accepted range: 0 to 99.",
                    OnTextChanged = OnTargetChanged
                }.SetKleiBlueStyle()
                    .SetMinWidthInCharacters(4)
                    .AddOnRealize(realized => targetField = realized));
            panel.AddChild(new PLabel("Status")
            {
                Text = "Alive: 0 • queued here: 0"
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
            base.ClearTarget();
        }

        private void OnEnabledChanged(GameObject _, int checkState)
        {
            if (!refreshing)
            {
                controller?.SetEnabled(checkState == PCheckBox.STATE_CHECKED);
                RefreshStatus();
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
                RefreshStatus();
            }
        }

        private void Refresh()
        {
            if (controller == null || enabledCheckbox == null || targetField == null)
            {
                return;
            }

            refreshing = true;
            PCheckBox.SetCheckState(
                enabledCheckbox,
                controller.Enabled
                    ? PCheckBox.STATE_CHECKED
                    : PCheckBox.STATE_UNCHECKED);
            targetField.GetComponent<TMP_InputField>()?.SetTextWithoutNotify(
                controller.TargetCount.ToString(CultureInfo.InvariantCulture));
            refreshing = false;
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            if (controller != null && statusLabel != null)
            {
                statusLabel.SetText(
                    $"Alive: {controller.GetLiveFlydoCount()} • queued here: {controller.GetManagedQueueCount()}");
            }
        }
    }
}
