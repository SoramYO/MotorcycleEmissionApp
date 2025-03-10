using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Common.Utils
{
    public class NavigationService
    {
        public void NavigateTo(Grid contentArea, UserControl view)
        {
            // Xóa tất cả nội dung hiện tại
            contentArea.Children.Clear();
            
            // Thêm view mới
            contentArea.Children.Add(view);
        }
    }
}
