﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface ICheckInService
    {
        Task<bool> CheckInParticipant(Guid participantId, Guid eventId);
        Task<bool> CheckInByQRCode(string qrCodeData);
    }
}
