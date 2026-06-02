import json
import os
import boto3
from fpdf import FPDF
from datetime import datetime
import logging

s3 = boto3.client('s3')
events = boto3.client('events')
logger = logging.getLogger()
logger.setLevel(logging.INFO)

def send_status_update(report_id, status, url=None):
    detail = {
        "metadata": {
            "version": "1.0",
            "created_at": datetime.utcnow().isoformat(),
            "domain": {
                "name": "TLAs",
                "subdomain": "report_generation",
                "service": "TLAReports",
                "category": "domain_event",
                "event": "TLAReport_Changed"
            }
        },
        "data": {
            "reportId": report_id,
            "status": status,
            "url": url
        }
    }

    events.put_events(
        Entries=[
            {
                'Source': 'tla-reports',
                'DetailType': 'TLAReport_Changed',
                'Detail': json.dumps(detail),
                'EventBusName': os.environ['TLA_EVENT_BUS_NAME']
            }
        ]
    )


def save_pdf_to_s3(pdf, report_id):
    pdf_bytes = bytes(pdf.output())

    bucket_name = os.environ['TLA_REPORT_BUCKET']
    file_key = f"reports/{report_id}.pdf"
    
    s3.put_object(
        Bucket=bucket_name,
        Key=file_key,
        Body=pdf_bytes,
        ContentType='application/pdf'
    )

    region = os.environ.get('AWS_REGION', 'us-east-1')
    return f"https://{bucket_name}.s3.{region}.amazonaws.com/{file_key}"


def create_pdf(report_id, tla_groups):
    pdf = FPDF()
    pdf.add_page()
    pdf.set_font("helvetica", "B", 16)
    pdf.cell(0, 10, f"TLA Report: {report_id}", ln=True, align="C")
    pdf.set_font("helvetica", size=10)
    pdf.cell(0, 10, f"Generated at: {datetime.utcnow().isoformat()}", ln=True, align="C")
    pdf.ln(10)

    for group in tla_groups:
        pdf.set_font("helvetica", "B", 12)
        pdf.cell(0, 10, f"Group: {group.get('name')}", ln=True)
        pdf.set_font("helvetica", "I", 10)
        pdf.multi_cell(0, 5, f"Description: {group.get('description')}")
        pdf.ln(2)

        # Table Header
        pdf.set_font("helvetica", "B", 10)
        pdf.cell(40, 8, "TLA Name", border=1)
        pdf.cell(100, 8, "Meaning", border=1)
        pdf.cell(50, 8, "Status", border=1, ln=True)

        # Table Rows
        pdf.set_font("helvetica", size=9)
        for tla in group.get('tlAs', []):
            pdf.cell(40, 8, str(tla.get('name')), border=1)
            pdf.cell(100, 8, str(tla.get('meaning')), border=1)
            pdf.cell(50, 8, str(tla.get('status')), border=1, ln=True)
        pdf.ln(10)

    return save_pdf_to_s3(pdf, report_id)


def generate_report(event, _):
    logger.info("Incoming event: %s", json.dumps(event, default=str))

    record = event['Records'][0]

    body = json.loads(record['body'])
    logger.info("Parsed body: %s", json.dumps(body, default=str))
    
    report_data = body.get('data', {})
    report_id = report_data.get('reportId')
    tla_groups = report_data.get('tlaGroups', [])

    send_status_update(report_id, "Running")

    url = create_pdf(report_id, tla_groups)

    send_status_update(report_id, "Finished", url)

    return {"status": "success", "reportId": report_id}