import json
import hashlib
import boto3
import time
from datetime import datetime, timedelta
import qrcode
import io
import os

s3_client = boto3.client('s3')
dynamodb = boto3.resource('dynamodb')

BUCKET_NAME = os.environ.get('BUCKET_NAME')
TABLE_NAME = os.environ.get('TABLE_NAME')
MAX_URL_LENGTH = 2048

def lambda_handler(event, context):
    try:
        body = json.loads(event.get('body', '{}'))
        url = body.get('url', '').strip()

        if not url:
            return response(400, 'URL cannot be empty.')
            
        if not (url.startswith('http://') or url.startswith('https://')):
            return response(400, 'URL must start with http:// or https://.')
            
        if len(url) > MAX_URL_LENGTH:
            return response(400, f'URL exceeds the limit of {MAX_URL_LENGTH} characters.')

        url_hash = hashlib.sha256(url.encode('utf-8')).hexdigest()
        file_extension = '.png'
        file_name = f"{url_hash}{file_extension}"

        qr = qrcode.QRCode(version=1, box_size=10, border=4)
        qr.add_data(url)
        qr.make(fit=True)
        img = qr.make_image(fill_color="black", back_color="white")

        img_byte_arr = io.BytesIO()
        img.save(img_byte_arr, format='PNG')
        img_bytes = img_byte_arr.getvalue()
        file_size = len(img_bytes)

        now = datetime.utcnow()
        expiration_date = now + timedelta(days=30) 

        table = dynamodb.Table(TABLE_NAME)
        table.put_item(
            Item={
                'id': url_hash,
                'url': url,
                'file_name': file_name,
                'file_extension': file_extension,
                'file_size_bytes': file_size,
                'created_at': now.isoformat(),
                'expires_at': int(expiration_date.timestamp())
            }
        )

        s3_client.put_object(
            Bucket=BUCKET_NAME,
            Key=file_name,
            Body=img_bytes,
            ContentType='image/png'
        )

        public_url = f"https://{BUCKET_NAME}.s3.amazonaws.com/{file_name}"

        return response(200, 'Success', {'qr_code_url': public_url})

    except Exception as e:
        print(f"Error: {str(e)}")
        return response(500, 'Internal server error.')

def response(status_code, message, data=None):
    body = {'message': message}
    if data:
        body.update(data)
    return {
        'statusCode': status_code,
        'headers': {'Content-Type': 'application/json'},
        'body': json.dumps(body)
    }