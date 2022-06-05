using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eugene_Ambilight.Enums
{
    public enum WindowShowing
    {
        /* Device adding */
        /// <summary>
        /// Окно с выбором метода поиска устройства.
        /// </summary>
        ChoosingAddingMethodDevice,

        /// <summary>
        /// Окно с ручным добавлением по IP.
        /// </summary>
        FindDeviceManual,

        /// <summary>
        /// Окно с автоматическим поиском устройства.
        /// </summary>
        FindDeviceAuto,

        /// <summary>
        /// Окно с подтверждением добавления устройства.
        /// </summary>
        ConfirmingFindedDevice,

        /* Main */
        /// <summary>
        /// Окно с выбором расположения светодиодов.
        /// </summary>
        ChoosingLocationLEDs,

        /// <summary>
        /// Окно управления устройством.
        /// </summary>
        DeviceManagement
    }
}
