
import React from "react";
import { Resume } from "@/types";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { useToast } from "@/components/ui/use-toast";
import { Link } from "react-router-dom";
import { Calendar, FileText, Link as LinkIcon, Mail, Phone } from "lucide-react";

interface ResumePreviewProps {
  resume: Resume;
}

const ResumePreview: React.FC<ResumePreviewProps> = ({ resume }) => {
  const { toast } = useToast();

  const copyLink = () => {
    navigator.clipboard.writeText(resume.shareableLink);
    toast({
      title: "Link copied",
      description: "The resume link has been copied to your clipboard",
    });
  };

  return (
    <div className="max-w-4xl mx-auto">
      <Card className="mb-8">
        <CardContent className="p-6">
          <div className="flex flex-col md:flex-row items-start md:items-center justify-between mb-6">
            <div>
              <h2 className="text-3xl font-bold text-primary">
                {resume.personalInfo.fullName}
              </h2>
              <p className="text-lg text-gray-600 mt-1">
                {resume.personalInfo.title}
              </p>
            </div>
            <div className="flex flex-col space-y-2 mt-4 md:mt-0">
              <div className="flex items-center">
                <Mail className="h-4 w-4 mr-2 text-primary" />
                <span>{resume.personalInfo.email}</span>
              </div>
              <div className="flex items-center">
                <Phone className="h-4 w-4 mr-2 text-primary" />
                <span>{resume.personalInfo.phone}</span>
              </div>
              <div className="flex items-center">
                <FileText className="h-4 w-4 mr-2 text-primary" />
                <span>{resume.personalInfo.address}</span>
              </div>
            </div>
          </div>

          <div className="resume-preview-section">
            <h3>Professional Summary</h3>
            <p className="mt-2 text-gray-700">{resume.personalInfo.summary}</p>
          </div>

          {resume.experience.length > 0 && (
            <div className="resume-preview-section">
              <h3>Work Experience</h3>
              <div className="space-y-6 mt-4">
                {resume.experience.map((exp) => (
                  <div key={exp.id} className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    <div className="md:col-span-1">
                      <p className="text-sm text-gray-500 flex items-center">
                        <Calendar className="h-3 w-3 mr-1" />
                        {exp.startDate} - {exp.current ? "Present" : exp.endDate}
                      </p>
                      <p className="text-sm text-gray-500 mt-1">{exp.location}</p>
                    </div>
                    <div className="md:col-span-3">
                      <h4 className="font-semibold">{exp.position}</h4>
                      <p className="text-primary-700">{exp.company}</p>
                      <p className="mt-2 text-gray-700">{exp.description}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {resume.education.length > 0 && (
            <div className="resume-preview-section">
              <h3>Education</h3>
              <div className="space-y-6 mt-4">
                {resume.education.map((edu) => (
                  <div key={edu.id} className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    <div className="md:col-span-1">
                      <p className="text-sm text-gray-500 flex items-center">
                        <Calendar className="h-3 w-3 mr-1" />
                        {edu.startDate} - {edu.current ? "Present" : edu.endDate}
                      </p>
                    </div>
                    <div className="md:col-span-3">
                      <h4 className="font-semibold">{edu.degree}</h4>
                      <p className="text-primary-700">{edu.institution}</p>
                      <p className="text-gray-600">{edu.fieldOfStudy}</p>
                      {edu.description && <p className="mt-2 text-gray-700">{edu.description}</p>}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {resume.skills.length > 0 && (
            <div className="resume-preview-section">
              <h3>Skills</h3>
              <div className="flex flex-wrap gap-2 mt-3">
                {resume.skills.map((skill, index) => (
                  <span
                    key={index}
                    className="bg-secondary text-secondary-foreground px-3 py-1 rounded-full text-sm"
                  >
                    {skill}
                  </span>
                ))}
              </div>
            </div>
          )}

          {resume.languages.length > 0 && (
            <div className="resume-preview-section">
              <h3>Languages</h3>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4 mt-3">
                {resume.languages.map((lang, index) => (
                  <div key={index} className="flex justify-between">
                    <span>{lang.language}</span>
                    <span className="text-gray-500">{lang.proficiency}</span>
                  </div>
                ))}
              </div>
            </div>
          )}

          {(resume.personalInfo.linkedin || resume.personalInfo.github || resume.personalInfo.website) && (
            <div className="resume-preview-section">
              <h3>Links</h3>
              <div className="space-y-2 mt-3">
                {resume.personalInfo.linkedin && (
                  <div className="flex items-center">
                    <LinkIcon className="h-4 w-4 mr-2 text-primary" />
                    <a
                      href={resume.personalInfo.linkedin}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-primary hover:underline"
                    >
                      LinkedIn
                    </a>
                  </div>
                )}
                {resume.personalInfo.github && (
                  <div className="flex items-center">
                    <LinkIcon className="h-4 w-4 mr-2 text-primary" />
                    <a
                      href={resume.personalInfo.github}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-primary hover:underline"
                    >
                      GitHub
                    </a>
                  </div>
                )}
                {resume.personalInfo.website && (
                  <div className="flex items-center">
                    <LinkIcon className="h-4 w-4 mr-2 text-primary" />
                    <a
                      href={resume.personalInfo.website}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-primary hover:underline"
                    >
                      Personal Website
                    </a>
                  </div>
                )}
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      <div className="bg-secondary p-6 rounded-lg mb-8">
        <h3 className="text-lg font-semibold mb-4">Share this resume with employers</h3>
        <div className="flex flex-col md:flex-row space-y-3 md:space-y-0 md:space-x-3">
          <Input
            value={resume.shareableLink}
            readOnly
            className="flex-1 bg-white"
          />
          <Button onClick={copyLink}>Copy Link</Button>
        </div>
        <p className="text-sm text-gray-600 mt-3">
          Companies can view your resume using this link without requiring login credentials.
        </p>
      </div>

      <div className="flex justify-end space-x-4 mb-10">
        <Link to="/dashboard">
          <Button variant="outline">Back to Dashboard</Button>
        </Link>
      </div>
    </div>
  );
};

export default ResumePreview;
