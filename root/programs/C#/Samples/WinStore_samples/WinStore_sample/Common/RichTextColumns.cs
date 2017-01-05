using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;

namespace WinStore_sample.Common
{
    /// <summary>
    /// <see cref="RichTextBlock"/> のラッパーは、使用可能なコンテンツに合わせて、
    /// 必要なオーバーフロー列を追加で作成します。
    /// </summary>
    /// <example>
    /// 以下では、400 ピクセル幅の列に 50 ピクセルの余白が指定されたコレクションを作成します。
    /// これには、データ バインドされた任意のコンテンツが含まれます:
    /// <code>
    /// <RichTextColumns>
    ///     <RichTextColumns.ColumnTemplate>
    ///         <DataTemplate>
    ///             <RichTextBlockOverflow Width="400" Margin="50,0,0,0"/>
    ///         </DataTemplate>
    ///     </RichTextColumns.ColumnTemplate>
    ///     
    ///     <RichTextBlock Width="400">
    ///         <Paragraph>
    ///             <Run Text="{Binding Content}"/>
    ///         </Paragraph>
    ///     </RichTextBlock>
    /// </RichTextColumns>
    /// </code>
    /// </example>
    /// <remarks>通常、バインドされていない領域で必要なすべての列を作成できる、
    /// 水平方向のスクロール領域で使用されます。垂直方向のスクロール領域で使用する場合、
    /// 列を追加で作成することはできません。</remarks>
    [Windows.UI.Xaml.Markup.ContentProperty(Name = "RichTextContent")]
    public sealed class RichTextColumns : Panel
    {
        /// <summary>
        /// <see cref="RichTextContent"/> 依存関係プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty RichTextContentProperty =
            DependencyProperty.Register("RichTextContent", typeof(RichTextBlock),
            typeof(RichTextColumns), new PropertyMetadata(null, ResetOverflowLayout));

        /// <summary>
        /// <see cref="ColumnTemplate"/> 依存関係プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty ColumnTemplateProperty =
            DependencyProperty.Register("ColumnTemplate", typeof(DataTemplate),
            typeof(RichTextColumns), new PropertyMetadata(null, ResetOverflowLayout));

        /// <summary>
        /// <see cref="RichTextColumns"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public RichTextColumns()
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
        }

        /// <summary>
        /// 最初のリッチ テキスト コンテンツを 1 つ目の列として使用するように取得または設定します。
        /// </summary>
        public RichTextBlock RichTextContent
        {
            get { return (RichTextBlock)GetValue(RichTextContentProperty); }
            set { SetValue(RichTextContentProperty, value); }
        }

        /// <summary>
        /// 追加の <see cref="RichTextBlockOverflow"/> インスタンスを
        /// 作成するために使用するテンプレートを取得または設定します。
        /// </summary>
        public DataTemplate ColumnTemplate
        {
            get { return (DataTemplate)GetValue(ColumnTemplateProperty); }
            set { SetValue(ColumnTemplateProperty, value); }
        }

        /// <summary>
        /// 列のレイアウトを再作成するため、コンテンツまたはオーバーフローのテンプレートを変更するときに呼び出されます。
        /// </summary>
        /// <param name="d">変更が発生した <see cref="RichTextColumns"/> の
        /// インスタンス。</param>
        /// <param name="e">特定の変更を説明するイベント データ。</param>
        private static void ResetOverflowLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 大幅な変更が行われた場合は、最初から列のレイアウトをビルドし直します
            var target = d as RichTextColumns;
            if (target != null)
            {
                target._overflowColumns = null;
                target.Children.Clear();
                target.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 既に作成されたオーバーフロー列を一覧表示します。
        /// 最初の子として RichTextBlock を含む <see cref="Panel.Children"/> コレクションのインスタンスは
        /// 1:1 の関係を保持する必要があります。
        /// </summary>
        private List<RichTextBlockOverflow> _overflowColumns = null;

        /// <summary>
        /// 追加のオーバーフロー列が必要かどうか、および既存の列を削除できるかどうかを
        /// 指定します。
        /// </summary>
        /// <param name="availableSize">空き領域のサイズは、作成できる追加の列の
        /// 数の制限に使用されます。</param>
        /// <returns>元のコンテンツと追加の列を合わせた最終的なサイズ。</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.RichTextContent == null) return new Size(0, 0);

            // RichTextBlock を子に指定するようにします。このとき、
            // 未完了であることを示すため、追加の列の一覧の不足箇所を
            // 使用します
            if (this._overflowColumns == null)
            {
                Children.Add(this.RichTextContent);
                this._overflowColumns = new List<RichTextBlockOverflow>();
            }

            // 最初は元の RichTextBlock コンテンツを基準にします
            this.RichTextContent.Measure(availableSize);
            var maxWidth = this.RichTextContent.DesiredSize.Width;
            var maxHeight = this.RichTextContent.DesiredSize.Height;
            var hasOverflow = this.RichTextContent.HasOverflowContent;

            // オーバーフロー列を十分に確保します
            int overflowIndex = 0;
            while (hasOverflow && maxWidth < availableSize.Width && this.ColumnTemplate != null)
            {
                // 既存のオーバーフロー列がなくなるまで使用した後、
                // 指定のテンプレートから作成します
                RichTextBlockOverflow overflow;
                if (this._overflowColumns.Count > overflowIndex)
                {
                    overflow = this._overflowColumns[overflowIndex];
                }
                else
                {
                    overflow = (RichTextBlockOverflow)this.ColumnTemplate.LoadContent();
                    this._overflowColumns.Add(overflow);
                    this.Children.Add(overflow);
                    if (overflowIndex == 0)
                    {
                        this.RichTextContent.OverflowContentTarget = overflow;
                    }
                    else
                    {
                        this._overflowColumns[overflowIndex - 1].OverflowContentTarget = overflow;
                    }
                }

                // 新しい列を基準にして、必要に応じて繰り返しの設定を行います
                overflow.Measure(new Size(availableSize.Width - maxWidth, availableSize.Height));
                maxWidth += overflow.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, overflow.DesiredSize.Height);
                hasOverflow = overflow.HasOverflowContent;
                overflowIndex++;
            }

            // 不要な列をオーバーフロー チェーンから切断し、列のプライベート リストから削除して、
            // 子として削除します
            if (this._overflowColumns.Count > overflowIndex)
            {
                if (overflowIndex == 0)
                {
                    this.RichTextContent.OverflowContentTarget = null;
                }
                else
                {
                    this._overflowColumns[overflowIndex - 1].OverflowContentTarget = null;
                }
                while (this._overflowColumns.Count > overflowIndex)
                {
                    this._overflowColumns.RemoveAt(overflowIndex);
                    this.Children.RemoveAt(overflowIndex + 1);
                }
            }

            // 最終的に決定したサイズを報告します
            return new Size(maxWidth, maxHeight);
        }

        /// <summary>
        /// 元のコンテンツと追加されたすべての列を整列します。
        /// </summary>
        /// <param name="finalSize">中で子を整列する必要がある領域のサイズを
        /// 定義します。</param>
        /// <returns>子が実際に必要とする領域のサイズ。</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double maxWidth = 0;
            double maxHeight = 0;
            foreach (var child in Children)
            {
                child.Arrange(new Rect(maxWidth, 0, child.DesiredSize.Width, finalSize.Height));
                maxWidth += child.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }
            return new Size(maxWidth, maxHeight);
        }
    }
}
