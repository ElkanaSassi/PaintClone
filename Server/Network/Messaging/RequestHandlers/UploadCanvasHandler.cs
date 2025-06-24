using Server.Logger;
using SharedModels.CommunicationModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.Network.Messaging.RequestHandlers
{
    public class UploadCanvasHandler : RequestHandler
    {
        public MessageType Type { get { return MessageType.UploadCanvas; } }

        public async Task HandleAsync(RequestInfo request, NetworkStream stream, List<string> openFiles)
        {
            // -the data strcture of Upload request-
            // 0-4      -> (X) file name length.
            // 4-X      -> file name.
            // X-END    -> shapes in Json presentation. 
            int fileNameLength = BitConverter.ToInt32(request.Data, 0); // 32bit - first 4 bytes.
            string fileName = Encoding.ASCII.GetString(request.Data, 4, fileNameLength);
            string shapeData = Encoding.ASCII.GetString(request.Data.Skip(4 + fileNameLength).ToArray());

            ResponseInfo response = StoreShapesInServer(fileName, shapeData);
            ServerLogger.Log(request.MessageType, request.From, response.IsSuccess);

            await ResponseBuilder.SendResponseToClientAsync(response, stream);
        }


        public ResponseInfo StoreShapesInServer(string fileName, string shapes)
        {
           

            try
            {
                // copmlete relative path
                string completePath = Path.Combine(LocalModels.LocalModels.canvasDirectory, fileName + ".json");

                using (FileStream Shapesfile = File.Create(completePath))
                {
                    Shapesfile.Write(Encoding.UTF8.GetBytes(shapes));
                }

                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    MainWindow.Instance.FileNames.Add(fileName);
                //});

                return new ResponseInfo { IsSuccess = true, Message = "Shapes were successfully stored!!!" };
            }
            catch (Exception ex)
            {
                return new ResponseInfo { IsSuccess = false, Message = "Error occurred in the file storage procedure!" };
            }

        }

    }
}
