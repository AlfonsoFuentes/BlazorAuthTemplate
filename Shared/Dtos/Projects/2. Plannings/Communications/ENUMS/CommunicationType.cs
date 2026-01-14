using Shared.Attributtes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Shared.Enums
{
    // PMP: El "Cómo" se entrega la información
 // Asegúrate de tener el namespace correcto

    public enum ActionCategory
    {
        [Description("Not Selected")]
        [UiIcon("fas fa-minus-circle")]
        None = 0,

        // TÚ -> HACIA OTROS
        // Física: Escribes/Adjuntas algo y pulsas "Enviar".
        // Ej: Mandar un correo, subir un archivo, mandar un WhatsApp.
        [Description("Send / Deliver (Unidirectional)")]
        [UiIcon("fas fa-paper-plane")]
        Send = 1,

        // TÚ <-> CON OTROS
        // Física: Estás presente (voz/video/físico) interactuando.
        // Ej: Reunión de equipo, llamada telefónica, sesión de feedback.
        [Description("Meet / Discuss (Interactive)")]
        [UiIcon("fas fa-users")]
        Meet = 2,

        // TÚ -> HACIA EL SISTEMA
        // Física: Entras a una web o archivo y cambias un dato. No envías nada a nadie directamente.
        // Ej: Actualizar el Jira/Trello, refrescar un Excel compartido, cargar horas.
        [Description("Update System / Record")]
        [UiIcon("fas fa-save")]
        Update = 3
    }

    public enum ArtifactType
    {
        [Description("Not Selected")]
        [UiIcon("fas fa-minus-circle")]
        None = 0,

        // --- COSAS QUE SE ENVÍAN (SEND) ---

        [Description("Formal Report (PDF/Doc)")]
        [UiIcon("fas fa-file-alt")]
        Report = 1, // El clásico informe formal.

        [Description("Email Update")]
        [UiIcon("fas fa-envelope")]
        Email = 2, // Un correo de "Solo para informar".

        [Description("Presentation / Slide Deck")]
        [UiIcon("fas fa-chalkboard-teacher")]
        Presentation = 3, // Enviar el PPT antes o después.

        // --- LUGARES DONDE TE REÚNES (MEET) ---

        [Description("Meeting (Online/Offline)")]
        [UiIcon("fas fa-handshake")]
        Meeting = 4, // La reunión estándar agendada.

        [Description("Quick Call / Chat")]
        [UiIcon("fas fa-phone-alt")]
        Call = 5, // Llamada rápida sin agenda formal.

        // --- COSAS QUE ACTUALIZAS (UPDATE) ---

        [Description("Dashboard / KPI Tracker")]
        [UiIcon("fas fa-chart-line")]
        Dashboard = 6, // Entrar a PowerBI/Excel y meter los datos del día.

        [Description("Task Board (Jira/Trello)")]
        [UiIcon("fas fa-tasks")]
        TaskBoard = 7 // Actualizar el estado de tus tickets.
    }

    // El "Cuándo" (El disparador)
    public enum CommunicationTrigger
    {
        [Description("Periodic (Weekly/Monthly)")]
        [UiIcon("fas fa-calendar-alt")]
        Periodic = 0, // Icono de calendario: Es una rutina fija.

        [Description("On Task Start")]
        [UiIcon("fas fa-hourglass-start")]
        TaskStart = 1, // Icono de reloj de arena empezando.

        [Description("On Task Completion")]
        [UiIcon("fas fa-check-circle")]
        TaskEnd = 2, // Icono de Check verde: Terminado.

        [Description("While Active (Ongoing)")]
        [UiIcon("fas fa-running")]
        WhileTaskActive = 3 // Icono de persona corriendo: Mientras dure.
    }

    // Para clasificar el artefacto

}
