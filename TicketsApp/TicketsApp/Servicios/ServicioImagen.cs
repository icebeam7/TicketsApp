﻿using System;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;

namespace TicketsApp.Servicios
{
    public static class ServicioImagen
    {
        public static async Task<MediaFile> TomarFoto()
        {
            MediaFile foto = null;

            try
            {
                await CrossMedia.Current.Initialize();

                if (CrossMedia.Current.IsCameraAvailable 
                    || CrossMedia.Current.IsTakePhotoSupported)
                {
                    foto = await CrossMedia.Current.PickPhotoAsync();
                }
            }
            catch (Exception ex)
            {

            }

            return foto;
        }
    }
}
