﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.CommunicationModels
{
    public enum MessageType
    {
        LoadCanvas,
        UploadCanvas,
        FileNameValidation,
        GetStoredFiles
    }
}
