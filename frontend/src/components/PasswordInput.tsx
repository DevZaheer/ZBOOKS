import { useState } from 'react'

interface PasswordInputProps {
  id: string
  label: string
  value: string
  onChange: (value: string) => void
  autoComplete: string
  required?: boolean
  minLength?: number
  hint?: string
}

function PasswordInput({ id, label, value, onChange, autoComplete, required = true, minLength, hint }: PasswordInputProps) {
  const [visible, setVisible] = useState(false)

  return (
    <div className="field-label">
      <label htmlFor={id}>{label}</label>
      <div className="password-input">
        <input
          id={id}
          type={visible ? 'text' : 'password'}
          required={required}
          minLength={minLength}
          autoComplete={autoComplete}
          value={value}
          onChange={(event) => onChange(event.target.value)}
        />
        <button
          className="password-toggle"
          type="button"
          aria-label={visible ? `Hide ${label.toLowerCase()}` : `Show ${label.toLowerCase()}`}
          aria-pressed={visible}
          onClick={() => setVisible((current) => !current)}
        >
          <svg viewBox="0 0 24 24" aria-hidden="true">
            <path d="M2 12s3.6-7 10-7 10 7 10 7-3.6 7-10 7S2 12 2 12Z" />
            <circle cx="12" cy="12" r="3" />
            {visible && <path d="m4 4 16 16" />}
          </svg>
        </button>
      </div>
      {hint && <small>{hint}</small>}
    </div>
  )
}

export default PasswordInput
