locals {
  services = toset([
    "api-gateway",
    "report",
    "verification",
    "assignment",
    "timeline",
    "notification",
  ])
}

resource "aws_ecr_repository" "repos" {
  for_each     = local.services
  name         = "urbanfix-${each.key}"
  force_delete = true

  tags = { Project = "UrbanFix" }
}
