using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MenuInDesignWithColorLabel
{
    //from https://jengting.blogspot.com/2020/11/UserControl-Smart-Tag-Panel.html

    /// <summary>
    /// 標準控件 Label 延伸，新增 ColorLock Property 來使用
    /// </summary>
    [Designer(typeof(ColorLabelControlDesigner))]
    public class ColorLabel : Label
    {
        public bool LockedColor { get; set; } = false;

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                if (LockedColor)
                    return;
                else
                    base.BackColor = value;
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                if (LockedColor)
                    return;
                else
                    base.ForeColor = value;
            }
        }
    }
    public class ColorLabelActionList : DesignerActionList
    {
        private ColorLabel colorLabel;

        private DesignerActionUIService designerActionUISvc = null;

        public ColorLabelActionList(IComponent component) : base(component)
        {
            this.colorLabel = component as ColorLabel;

            // DesignerActionUIService 可以用來更新智能面板
            this.designerActionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }

        /// <summary>
        /// DesignerActionUIService 使用來更新智能面板，在本範例上 [鎖定顏色] 和 [反轉前後背景顏色] 會用上
        /// </summary>
        private void SmartTagPanelRefresh()
        {
            this.designerActionUISvc.Refresh(this.Component);
        }

        /// <summary>
        /// 取得 ColorLabel Property
        /// </summary>
        /// <param name="propName">Property Name</param>
        /// <returns>PropertyDescriptor</returns>
        private PropertyDescriptor GetPropertyByName(String propName)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(colorLabel)[propName];
            if (null == prop)
                throw new ArgumentException($"ColorLabel 屬性：{propName} 沒有找到");
            else
                return prop;
        }

        #region DesignerActionPropertyItem：智能面版上的控件

        public string Text
        {
            get
            {
                return colorLabel.Text;
            }
            set
            {
                GetPropertyByName(nameof(this.Text)).SetValue(colorLabel, value);
            }
        }

        public Color BackColor
        {
            get
            {
                return colorLabel.BackColor;
            }
            set
            {
                GetPropertyByName(nameof(this.BackColor)).SetValue(colorLabel, value);
            }
        }

        public Color ForeColor
        {
            get
            {
                return colorLabel.ForeColor;
            }
            set
            {
                GetPropertyByName(nameof(this.ForeColor)).SetValue(colorLabel, value);
            }
        }

        public bool LockedColor
        {
            get
            {
                return colorLabel.LockedColor;
            }
            set
            {
                GetPropertyByName(nameof(this.LockedColor)).SetValue(colorLabel, value);
                SmartTagPanelRefresh();
            }
        }
        #endregion

        #region DesignerActionMethodItem：智能面板上的方法
        /// <summary>
        /// 反轉前後背景顏色
        /// </summary>
        public void InvertColors()
        {
            Color currentBackColor = colorLabel.BackColor;
            Color currentForeColor = colorLabel.ForeColor;

            BackColor = currentForeColor;
            ForeColor = currentBackColor;

            SmartTagPanelRefresh();
        }
        #endregion

        /// <summary>
        /// 智能面板內容
        /// </summary>
        /// <returns></returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();

            // DesignerActionHeaderItem 使用
            string Header_Appearance = "外觀";
            string Header_Information = "控件資訊";

            // 定義 Header
            items.Add(new DesignerActionHeaderItem(Header_Appearance));
            items.Add(new DesignerActionHeaderItem(Header_Information));

            items.Add(new DesignerActionPropertyItem(
                            nameof(this.LockedColor), // 綁定的 Property
                            "鎖定顏色", // 文字說明
                            Header_Appearance, // Category：輸入 Header 名稱
                            "鎖定顏色相關屬性" // 提示說明
                            ));

            if (!LockedColor)
            {
                items.Add(new DesignerActionPropertyItem(
                                 nameof(this.BackColor),
                                 "背景顏色",
                                 Header_Appearance,
                                 "選擇背景顏色"));

                items.Add(new DesignerActionPropertyItem(
                                 nameof(this.ForeColor),
                                 "前景顏色",
                                 Header_Appearance,
                                 "選擇前景顏色"));

                // DesignerActionMethodItem 也可以加入 context menu 內 例如：designer verb
                items.Add(new DesignerActionMethodItem(this,
                                 nameof(this.InvertColors), // Method 名稱
                                 "反轉前景和背景顏色",
                                 Header_Appearance,
                                 "反轉前景和背景顏色",
                                  true));
            }

            items.Add(new DesignerActionPropertyItem(
                nameof(this.Text),
                "標籤文字",
                Header_Appearance,
                "設定標籤文字"));

            // ColorLabel 相關資訊
            items.Add(new DesignerActionTextItem($"位置：{colorLabel.Location}", Header_Information));
            items.Add(new DesignerActionTextItem($"大小：{colorLabel.Size}", Header_Information));

            return items;
        }
    }


    /// <summary>
    /// ColorLabel 的智能標籤面板，必須引用 System.Design.dll
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class ColorLabelControlDesigner : ControlDesigner
    {
        private DesignerActionListCollection actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    // 加入智能面板內容
                    actionLists = new DesignerActionListCollection();
                    actionLists.Add(new ColorLabelActionList(this.Component));
                }
                return actionLists;
            }
        }
    }
}
