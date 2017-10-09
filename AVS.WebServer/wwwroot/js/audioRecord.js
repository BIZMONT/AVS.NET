//Microphone configuration

var proceeding = false;
var audio = document.getElementById('response') || new Audio();

function configureMicrophone() {
    if (!navigator.getUserMedia) {
        navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia ||
            navigator.mozGetUserMedia || navigator.msGetUserMedia;
    }

    if (navigator.getUserMedia) {
        navigator.getUserMedia({ audio: true },
            getMediaCallback,
            function (e) { $("#status").html('Error capturing audio.'); }
        );
    }
    else { $("#status").html('getUserMedia not supported in this browser.'); }
}


function getMediaCallback(stream) {
    var gain = start_microphone(stream);
    var recorder = configureRecorder(gain);

    $("#status").html("Hold spacebar to say something to Alexa");

    function startRecording()
    {
        if (!recorder.isRecording() && !proceeding) {
            $(".circle img").addClass("record");

            proceeding = true;

            audio.pause();

            recorder.startRecording();

            $("#status").html('Listerning<span class="dot">.</span><span class="dot">.</span><span class="dot">.</span>');
        }
    }

    function stopRecording()
    {
        if (recorder.isRecording()) {
            $(".circle img").removeClass("record");
            $("#status").html("Hold Spacebar to say something to Alexa");

            recorder.finishRecording();
        }
    }

    $(".circle.inner").on("touchstart", function () {
        startRecording();
    });

    $(".circle.inner").on("touchend", function () {
        stopRecording();
    });

    $(document).keydown(function (event) {
        if (event.which === 32) {
            event.preventDefault();

            startRecording();
        }
    });

    $(document).keyup(function (event) {
        if (event.which === 32) {
            event.preventDefault();

            stopRecording();
        }
    });
}

function start_microphone(stream) {
    var BUFF_SIZE = 2048;

    var audioContext = new AudioContext();

    var volume = audioContext.createGain();

    volume.gain.value = 1;//Volume

    var audioInput = audioContext.createMediaStreamSource(stream);
    audioInput.connect(volume);

    var recorder = audioContext.createScriptProcessor(BUFF_SIZE, 1, 1);

    volume.connect(recorder);
    recorder.connect(audioContext.destination); 

    return volume;
}

function process_microphone_buffer(event) {

    var i, N, inp, microphone_output_buffer;
    microphone_output_buffer = event.inputBuffer.getChannelData(0);
}
//End of microphone configuration

function configureRecorder(gain) {
    var audioRecorder = new WebAudioRecorder(gain, {
        workerDir: 'js/'
    });

    audioRecorder.setOptions({
        timeLimit: 60,
        encodeAfterRecord: false,
        progressInterval: 1000,
        wav: {
            mimeType: "audio/wav"
        }
    });

    audioRecorder.onTimeout = function (recorder) {
        recorder.finishRecording();
    };

    audioRecorder.onComplete = function (recorder, blob) {
        sendAudioToServer(blob);
    };

    audioRecorder.onError = function (recorder, message) {
        $("#status").html(message);
    };

    return audioRecorder
}

function sendAudioToServer(blob) {
    var url = (window.URL || window.webkitURL).createObjectURL(blob);

    var data = new FormData();
    data.append('fname', 'request.wav');
    data.append('audioRequest', blob);

    $("#status").html('Waiting for response<span class="dot">.</span><span class="dot">.</span><span class="dot">.</span>');
    $.ajax({
        url: "communicate",
        type: 'POST',
        data: data,
        contentType: false,
        processData: false,
        complete: function () {
            proceeding = false;
        },
        success: proceedResponse,
        error: function (err) {
            $("#status").html(err);
        }
    });
}

function proceedResponse(data) {
    if (!data.error && data.response) {
        try {
            var blob = b64toBlob(data.response, 'audio/wav');
            var objectUrl = URL.createObjectURL(blob);

            audio.src = objectUrl;
            audio.onload = function (evt) {
                URL.revokeObjectUrl(objectUrl);
            };
            audio.addEventListener('ended', function () {
                $(".circle.outer").removeClass("play");
                $("#status").html("Hold spacebar to say something to Alexa");
            });

            audio.addEventListener("play", function () {
                $(".circle.outer").addClass("play");
            });
            audio.play();
        }
        catch (err) {
            $(".circle.outer").removeClass("play");
            $("#status").html(err);
            return;
        }
    }
    $("#status").html(data.text);
}

function b64toBlob(b64Data, contentType, sliceSize) {
    contentType = contentType || '';
    sliceSize = sliceSize || 512;

    var byteCharacters = atob(b64Data);
    var byteArrays = [];

    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        var slice = byteCharacters.slice(offset, offset + sliceSize);

        var byteNumbers = new Array(slice.length);
        for (var i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        var byteArray = new Uint8Array(byteNumbers);

        byteArrays.push(byteArray);
    }

    var blob = new Blob(byteArrays, { type: contentType });
    return blob;
}

$(document).ready(function () {
    $("#status").html("Hold spacebar to say something to Alexa");
    configureMicrophone();
});