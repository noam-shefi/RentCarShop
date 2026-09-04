<%@ Page Title="התאמת רכב חכמה" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="CarMatch.aspx.cs" Inherits="CarShop.CarMatch" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="section">
        <h2>מצא את הרכב המושלם לטיול שלך</h2>

        <p style="color:var(--color-text-light);margin-bottom:20px;">
            ספר לנו קצת על הטיול המתוכנן שלך או לחץ על סמל המיקרופון כדי לדבר אלינו ישירות!
        </p>
    </div>

    <div class="form-container" style="max-width:650px;">

        <label>פרטי הטיול שלך</label>

        <asp:TextBox
            ID="txtTripDetails"
            runat="server"
            ClientIDMode="Static"
            TextMode="MultiLine"
            Rows="6"
            placeholder="לדוגמה: אנחנו משפחה עם שני ילדים קטנים, נוסעים לטיול קמפינג בצפון..."
            style="width:100%;resize:vertical;">
        </asp:TextBox>


        <!-- ============================= -->
        <!-- אזור הקלטת קול -->
        <!-- ============================= -->

        <div style="
            margin-top:15px;
            padding:15px;
            background:#f1f5f9;
            border-radius:8px;
            text-align:center;
        ">

            <!-- כפתור מיקרופון -->
            <button
                type="button"
                id="btnMic"
                onclick="toggleRecording()"
                aria-label="הקלטת קול"
                title="לחץ כדי לדבר"
                style="
                    width:44px;
                    height:44px;
                    padding:0;
                    display:inline-flex;
                    align-items:center;
                    justify-content:center;
                    background:#ffffff;
                    color:#111827;
                    border:1px solid #d1d5db;
                    border-radius:50%;
                    cursor:pointer;
                    box-shadow:0 2px 6px rgba(0,0,0,0.08);
                    transition:all 0.2s ease;
                "
                onmouseover="this.style.backgroundColor='#f3f4f6'; this.style.borderColor='#9ca3af';"
                onmouseout="this.style.backgroundColor='#ffffff'; this.style.borderColor='#d1d5db';"
            >
                <svg
                    width="21"
                    height="21"
                    viewBox="0 0 24 24"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                    aria-hidden="true"
                >
                    <path
                        d="M12 15C13.6569 15 15 13.6569 15 12V6C15 4.34315 13.6569 3 12 3C10.3431 3 9 4.34315 9 6V12C9 13.6569 10.3431 15 12 15Z"
                        stroke="currentColor"
                        stroke-width="1.8"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                    />
                    <path
                        d="M5 11C5 14.866 8.13401 18 12 18C15.866 18 19 14.866 19 11"
                        stroke="currentColor"
                        stroke-width="1.8"
                        stroke-linecap="round"
                    />
                    <path
                        d="M12 18V21"
                        stroke="currentColor"
                        stroke-width="1.8"
                        stroke-linecap="round"
                    />
                    <path
                        d="M9 21H15"
                        stroke="currentColor"
                        stroke-width="1.8"
                        stroke-linecap="round"
                    />
                </svg>
            </button>

            <!-- סטטוס -->
            <span
                id="micStatus"
                style="
                    display:block;
                    margin-top:8px;
                    font-weight:bold;
                    color:#475569;
                ">
                לחץ כדי להתחיל להקליט
            </span>


            <!-- ============================= -->
            <!-- Gemini Style Waveform -->
            <!-- ============================= -->

            <div
                id="waveform"
                style="
                    display:none;
                    height:55px;
                    margin:15px auto 0;
                    align-items:center;
                    justify-content:center;
                    gap:3px;
                ">

                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>
                <span class="wave"></span>

            </div>

        </div>


        <!-- ============================= -->
        <!-- Find Car Button -->
        <!-- ============================= -->

        <asp:Button
            ID="btnFindCar"
            runat="server"
            Text="מצא לי רכב מתאים"
            CssClass="btn btn-main"
            OnClick="btnFindCar_Click"
            style="margin-top:16px;width:100%;" />


        <!-- Status -->
        <asp:Label
            ID="lblStatus"
            runat="server"
            style="
                display:block;
                margin-top:12px;
                font-size:14px;
            ">
        </asp:Label>


        <!-- ============================= -->
        <!-- Results -->
        <!-- ============================= -->

        <div
            id="resultBox"
            runat="server"
            visible="false"
            style="
                margin-top:20px;
                padding:18px;
                background-color:#f8fafc;
                border:1px solid var(--color-gray-dark);
                border-radius:10px;
            ">

            <h3 style="
                margin-bottom:10px;
                color:var(--color-black);
            ">
                ההמלצה שלנו
            </h3>

            <asp:Label
                ID="lblResult"
                runat="server"
                style="
                    line-height:1.8;
                    display:block;
                ">
            </asp:Label>

        </div>

    </div>


    <!-- ============================= -->
    <!-- Waveform CSS -->
    <!-- ============================= -->

    <style>

        .wave {
            width:4px;
            height:8px;
            background:#64748b;
            border-radius:10px;
            transition:height .08s ease, background .15s ease;
        }

    </style>


    <!-- ============================= -->
    <!-- JavaScript -->
    <!-- ============================= -->

    <script>

        let mediaRecorder = null;
        let audioChunks = [];
        let isRecording = false;
        let fileExtension = "webm";

        let audioContext = null;
        let analyser = null;
        let microphoneSource = null;
        let animationId = null;
        let microphoneStream = null;
        let dataArray = null;


        // =========================================
        // START / STOP RECORDING
        // =========================================

        async function toggleRecording() {

            const micBtn = document.getElementById("btnMic");
            const micStatus = document.getElementById("micStatus");
            const waveform = document.getElementById("waveform");


            // =========================================
            // START
            // =========================================

            if (!isRecording) {

                try {

                    if (
                        !navigator.mediaDevices ||
                        !navigator.mediaDevices.getUserMedia
                    ) {

                        alert(
                            "הדפדפן שלך אינו תומך בהקלטת אודיו או שהחיבור אינו מאובטח. השתמש ב-HTTPS או localhost."
                        );

                        return;
                    }


                    // Ask for microphone permission
                    microphoneStream =
                        await navigator.mediaDevices.getUserMedia({
                            audio: true
                        });


                    // =========================================
                    // Audio Context
                    // =========================================

                    const AudioContext =
                        window.AudioContext ||
                        window.webkitAudioContext;


                    if (!AudioContext) {

                        alert(
                            "הדפדפן שלך אינו תומך ב-AudioContext."
                        );

                        return;
                    }


                    audioContext = new AudioContext();


                    if (audioContext.state === "suspended") {
                        await audioContext.resume();
                    }


                    // =========================================
                    // Analyzer
                    // =========================================

                    analyser =
                        audioContext.createAnalyser();

                    analyser.fftSize = 256;

                    analyser.smoothingTimeConstant = 0.8;


                    microphoneSource =
                        audioContext.createMediaStreamSource(
                            microphoneStream
                        );


                    microphoneSource.connect(
                        analyser
                    );


                    dataArray =
                        new Uint8Array(
                            analyser.fftSize
                        );


                    // =========================================
                    // MediaRecorder
                    // =========================================

                    let options = {};


                    if (
                        MediaRecorder.isTypeSupported(
                            "audio/webm"
                        )
                    ) {

                        options = {
                            mimeType: "audio/webm"
                        };

                        fileExtension = "webm";

                    }
                    else if (
                        MediaRecorder.isTypeSupported(
                            "audio/mp4"
                        )
                    ) {

                        options = {
                            mimeType: "audio/mp4"
                        };

                        fileExtension = "mp4";

                    }


                    mediaRecorder =
                        new MediaRecorder(
                            microphoneStream,
                            options
                        );


                    audioChunks = [];


                    // =========================================
                    // Audio Data
                    // =========================================

                    mediaRecorder.ondataavailable =
                        function (event) {

                            if (
                                event.data &&
                                event.data.size > 0
                            ) {

                                audioChunks.push(
                                    event.data
                                );

                            }

                        };


                    // =========================================
                    // STOP EVENT
                    // =========================================

                    mediaRecorder.onstop =
                        async function () {

                            micStatus.innerText =
                                "מעבד את ההקלטה ושולח ל-AI...";


                            const audioBlob =
                                new Blob(
                                    audioChunks,
                                    {
                                        type:
                                            options.mimeType ||
                                            "audio/webm"
                                    }
                                );


                            if (audioBlob.size === 0) {

                                alert(
                                    "לא נקלט שום שמע."
                                );

                                micStatus.innerText =
                                    "שגיאת מיקרופון";

                                return;
                            }


                            // =========================================
                            // Convert to Base64
                            // =========================================

                            const reader =
                                new FileReader();


                            reader.readAsDataURL(
                                audioBlob
                            );


                            reader.onloadend =
                                async function () {

                                    const base64String =
                                        reader.result.split(",")[1];


                                    try {

                                        const response =
                                            await fetch(
                                                "CarMatch.aspx/TranscribeAudio",
                                                {
                                                    method: "POST",

                                                    headers: {
                                                        "Content-Type":
                                                            "application/json; charset=utf-8"
                                                    },

                                                    body:
                                                        JSON.stringify({
                                                            base64Audio:
                                                                base64String,

                                                            fileExt:
                                                                fileExtension
                                                        })
                                                }
                                            );


                                        const result =
                                            await response.json();


                                        // =========================================
                                        // SUCCESS
                                        // =========================================

                                        if (response.ok) {

                                            document.getElementById(
                                                "txtTripDetails"
                                            ).value =
                                                result.d;

                                            micStatus.innerText =
                                                "התמלול הושלם בהצלחה!";

                                        }

                                        // =========================================
                                        // SERVER ERROR
                                        // =========================================

                                        else {

                                            alert(
                                                "שגיאה מהשרת: " +
                                                (
                                                    result.Message ||
                                                    "שגיאה לא ידועה"
                                                )
                                            );

                                            micStatus.innerText =
                                                "שגיאה בתמלול";
                                        }

                                    }

                                    // =========================================
                                    // NETWORK ERROR
                                    // =========================================

                                    catch (err) {

                                        console.error(err);

                                        alert(
                                            "שגיאת תקשורת מול השרת: " +
                                            err
                                        );

                                        micStatus.innerText =
                                            "שגיאת רשת";
                                    }
                                };
                        };


                    // =========================================
                    // IMPORTANT
                    // Set recording BEFORE waveform
                    // =========================================

                    isRecording = true;

                    mediaRecorder.start(250);

                    waveform.style.display = "flex";

                    micBtn.style.backgroundColor =
                        "#22c55e";

                    micBtn.style.transform =
                        "scale(1.15)";

                    micStatus.innerText =
                        "מקליט כעת... דבר ברור ולחץ שוב לסיום";


                    // Start waveform
                    updateWaveform();


                }

                catch (err) {

                    console.error(
                        "Microphone error:",
                        err
                    );

                    alert(
                        "אין גישה למיקרופון:\n" +
                        err.message
                    );
                }

            }


            // =========================================
            // STOP
            // =========================================

            else {

                isRecording = false;


                // Stop recorder
                if (
                    mediaRecorder &&
                    mediaRecorder.state !== "inactive"
                ) {

                    mediaRecorder.stop();

                }


                // Stop animation
                if (animationId) {

                    cancelAnimationFrame(
                        animationId
                    );

                    animationId = null;
                }


                // Stop microphone
                if (microphoneStream) {

                    microphoneStream
                        .getTracks()
                        .forEach(
                            track => track.stop()
                        );

                    microphoneStream = null;
                }


                // Disconnect microphone
                if (microphoneSource) {

                    try {
                        microphoneSource.disconnect();
                    }
                    catch (e) { }

                    microphoneSource = null;
                }


                // Close AudioContext
                if (
                    audioContext &&
                    audioContext.state !== "closed"
                ) {

                    try {
                        await audioContext.close();
                    }
                    catch (e) { }

                    audioContext = null;
                }


                analyser = null;


                // Reset UI
                waveform.style.display =
                    "none";

                micBtn.style.backgroundColor =
                    "#ef4444";

                micBtn.style.transform =
                    "scale(1)";

            }
        }


        // =========================================
        // WAVEFORM
        // =========================================

        function updateWaveform() {

            if (
                !isRecording ||
                !analyser ||
                !dataArray
            ) {

                return;
            }


            // Get microphone data
            analyser.getByteTimeDomainData(
                dataArray
            );


            let sum = 0;


            // Calculate RMS volume
            for (
                let i = 0;
                i < dataArray.length;
                i++
            ) {

                const sample =
                    (dataArray[i] - 128) / 128;

                sum +=
                    sample * sample;

            }


            const rms =
                Math.sqrt(
                    sum / dataArray.length
                );


            // Volume 0-1
            const volume =
                Math.min(
                    1,
                    rms * 6
                );


            const waves =
                document.querySelectorAll(
                    ".wave"
                );


            // =========================================
            // Animate each bar
            // =========================================

            waves.forEach(
                function (wave, index) {

                    const center =
                        Math.abs(
                            index -
                            (waves.length - 1) / 2
                        );


                    const falloff =
                        Math.max(
                            0.25,
                            1 -
                            center /
                            (waves.length / 2)
                        );


                    const variation =
                        0.7 +
                        Math.random() * 0.3;


                    const height =
                        8 +
                        (
                            volume *
                            42 *
                            falloff *
                            variation
                        );


                    wave.style.height =
                        height + "px";


                    // Change color
                    if (volume > 0.7) {

                        wave.style.backgroundColor =
                            "#ef4444";

                    }
                    else if (volume > 0.4) {

                        wave.style.backgroundColor =
                            "#eab308";

                    }
                    else {

                        wave.style.backgroundColor =
                            "#64748b";
                    }

                }
            );


            // Continue animation
            animationId =
                requestAnimationFrame(
                    updateWaveform
                );
        }

    </script>

</asp:Content>