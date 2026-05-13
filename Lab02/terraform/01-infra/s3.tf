resource "aws_s3_bucket" "report" {
  bucket        = "urbanfix-report-tf"
  force_destroy = true

  tags = { Project = "UrbanFix" }
}

resource "aws_s3_bucket_public_access_block" "report" {
  bucket = aws_s3_bucket.report.id

  block_public_acls       = false
  block_public_policy     = false
  ignore_public_acls      = false
  restrict_public_buckets = false
}
