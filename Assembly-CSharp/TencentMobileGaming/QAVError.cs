using System;

namespace TencentMobileGaming
{
	public class QAVError
	{
		public static int OK;

		public static int ERR_REPETITIVE_OPERATION = 1001;

		public static int ERR_EXCLUSIVE_OPERATION = 1002;

		public static int ERR_HAS_IN_THE_STATE = 1003;

		public static int ERR_INVALID_ARGUMENT = 1004;

		public static int ERR_TIMEOUT = 1005;

		public static int ERR_NOT_IMPLEMENTED = 1006;

		public static int ERR_NOT_IN_MAIN_THREAD = 1007;

		public static int ERR_CONTEXT_NOT_START = 1101;

		public static int ERR_ROOM_NOT_EXIST = 1201;

		public static int ERR_DEVICE_NOT_EXIST = 1301;

		public static int ERR_SERVER_FAILED = 10001;

		public static int ERR_SERVER_NO_PERMISSION = 10003;

		public static int ERR_SERVER_REQUEST_ROOM_ADDRESS_FAIL = 10004;

		public static int ERR_SERVER_CONNECT_ROOM_FAIL = 10005;

		public static int ERR_SERVER_FREE_FLOW_AUTH_FAIL = 10006;

		public static int ERR_SERVER_ROOM_DISSOLVED = 10007;

		public static int ERR_IMSDK_UNKNOWN = 6999;

		public static int ERR_IMSDK_TIMEOUT = 7000;

		public static int ERR_HTTP_REQ_FAIL = 7001;

		public static int AV_ERR_3DVOICE_ERR_FILE_DAMAGED = 7002;

		public static int AV_ERR_3DVOICE_ERR_NOT_INITED = 7003;

		public static int AV_ERR_NET_REQUEST_FALLED = 7004;

		public static int AV_ERR_CHARGE_OVERDUE = 7005;

		public static int AV_ERR_AUTH_FIALD = 7006;

		public static int AV_ERR_IN_OTHER_ROOM = 7007;

		public static int AV_ERR_DISSOLVED_OVERUSER = 7008;

		public static int AV_ERR_NO_PERMISSION = 7009;

		public static int AV_ERR_FILE_CANNOT_ACCESS = 7010;

		public static int AV_ERR_FILE_DAMAGED = 7011;

		public static int AV_ERR_SERVICE_NOT_OPENED = 7012;

		public static int ERR_3DVOICE_ERR_FILE_DAMAGED = 7002;

		public static int ERR_3DVOICE_ERR_NOT_INITED = 7003;

		public static int ERR_UNKNOWN = 65536;

		public static int ERR_ACC_OPENFILE_FAILED = 1953;

		public static int ERR_ACC_FILE_FORAMT_NOTSUPPORT = 1954;

		public static int ERR_ACC_DECODER_FAILED = 1955;

		public static int ERR_ACC_BAD_PARAM = 1956;

		public static int ERR_ACC_MEMORY_ALLOC_FAILED = 1957;

		public static int ERR_ACC_CREATE_THREAD_FAILED = 1958;

		public static int ERR_ACC_NOT_STARTED = 1959;

		public static int ERR_EFFECT_OPENFILE_FAILED = 4051;

		public static int ERR_EFFECT_FILE_FORAMT_NOTSUPPORT = 4052;

		public static int ERR_EFFECT_DECODER_FAILED = 4053;

		public static int ERR_EFFECT_BAD_PARAM = 4054;

		public static int ERR_EFFECT_MEMORY_ALLOC_FAILED = 4055;

		public static int ERR_EFFECT_CREATE_THREAD_FAILED = 4056;

		public static int ERR_EFFECT_STATE_ILLIGAL = 4057;

		public static int ERR_VOICE_RECORD_PARAM_NULL = 4097;

		public static int ERR_VOICE_RECORD_INIT_ERR = 4098;

		public static int ERR_VOICE_RECORD_RECORDING_ERR = 4099;

		public static int ERR_VOICE_RECORD_NODATA_ERR = 4100;

		public static int ERR_VOICE_RECORD_OPENFILE_ERR = 4101;

		public static int ERR_VOICE_RECORD_PERMISSION_MIC_ERR = 4102;

		public static int ERR_VOICE_RECORD_AUDIO_TOO_SHORT = 4103;

		public static int ERR_VOICE_RECORD_RECORD_NOT_START = 4104;

		public static int ERR_VOICE_UPLOAD_FILE_ACCESSERROR = 8193;

		public static int ERR_VOICE_UPLOAD_SIGN_CHECK_FAIL = 8194;

		public static int ERR_VOICE_UPLOAD_COS_INTERNAL_FAIL = 8195;

		public static int ERR_VOICE_UPLOAD_GET_TOKEN_NETWORK_FAIL = 8196;

		public static int ERR_VOICE_UPLOAD_SYSTEM_INNER_ERROR = 8197;

		public static int ERR_VOICE_UPLOAD_RSP_DATA_DECODE_FAIL = 8198;

		public static int ERR_VOICE_UPLOAD_APPINFO_UNSET = 8200;

		public static int ERR_VOICE_DOWNLOAD_FILE_ACCESSERROR = 12289;

		public static int ERR_VOICE_DOWNLOAD_SIGN_CHECK_FAIL = 12290;

		public static int ERR_VOICE_DOWNLOAD_COS_INTERNAL_FAIL = 12291;

		public static int ERR_VOICE_DOWNLOAD_REMOTEFILE_ACCESSERROR = 12292;

		public static int ERR_VOICE_DOWNLOAD_GET_SIGN_NETWORK_FAIL = 12293;

		public static int ERR_VOICE_DOWNLOAD_SYSTEM_INNER_ERROR = 12294;

		public static int ERR_VOICE_DOWNLOAD_GET_SIGN_RSP_DATA_DECODE_FAIL = 12295;

		public static int ERR_VOICE_DOWNLOAD_APPINFO_UNSET = 12297;

		public static int ERR_VOICE_PLAYER_INIT_ERR = 20481;

		public static int ERR_VOICE_PLAYER_PLAYING_ERR = 20482;

		public static int ERR_VOICE_PLAYER_PARAM_NULL = 20483;

		public static int ERR_VOICE_PLAYER_OPENFILE_ERR = 20484;

		public static int ERR_VOICE_PLAYER_NOT_START_ERR = 20485;

		public static int ERR_VOICE_S2T_INTERNAL_ERROR = 32769;

		public static int ERR_VOICE_S2T_NETWORK_FAIL = 32770;

		public static int ERR_VOICE_S2T_RSP_DATA_DECODE_FAIL = 32772;

		public static int ERR_VOICE_S2T_APPINFO_UNSET = 32774;

		public static int ERR_VOICE_STREAMIN_RECORD_SUC_REC_FAIL = 32775;

		public static int ERR_VOICE_S2T_SIGN_CHECK_FAIL = 32776;

		public static int ERR_VOICE_STREAMIN_UPLOADANDRECORD_SUC_REC_FAIL = 32777;

		public static int ERR_VOICE_S2T_PARAM_NULL = 32784;

		public static int ERR_VOICE_S2T_AUTO_SPEECH_REC_ERROR = 32785;

		public static int ERR_VOICE_ERR_VOICE_STREAMIN_RUNING_ERROR = 32786;
	}
}
